using API.Data.Entities;
using API.DTOs.UserDtos;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly IEmailSender _emailSender;
        private readonly JwtEmailSettings _emailJwtSettings;

        public AccountsController(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper,
            ICartService cartService,
            IOptions<JwtEmailSettings> emailJwtSettings,
            IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
            _cartService = cartService;
            _emailSender = emailSender;
            _emailJwtSettings = emailJwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(registerDto.Email);
            if (user != null)
            {
                return BadRequest("Email này đã được sử dụng!");
            }

            try
            {
                user = _mapper.Map<User>(registerDto);
                user.Id = Guid.NewGuid();
                user = await _unitOfWork.AccountRepository.RegisterAsync(user, registerDto.Password);
                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.TokenExpiryTime = DateTime.UtcNow.AddDays(7);

                // add new cart for user
                var userCart = new Cart
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id
                };
                await _unitOfWork.CartRepository.AddAsync(userCart);

                if (await _unitOfWork.SaveChangesAsync())
                {
                    await _emailSender.SendVerificationEmailAsync(user);

                    return Ok();
                }

                return BadRequest("Falied to register!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _unitOfWork.AccountRepository.AuthenticateAsync(loginDto);

            if (user is null) return BadRequest("Email hoặc mật khẩu không hợp lệ!");

            if (!user.IsAuthenticated) return Forbid();

            var claims = _tokenService.GenerateClaims(user);

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.TokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _unitOfWork.AccountRepository.Update(user);

            if(await _unitOfWork.SaveChangesAsync())
            {
                _tokenService.SetTokenInsideCookies(new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }, HttpContext);

                return Ok(_mapper.Map<UserDto>(user));
            }

            return BadRequest("Failed to login!");
        }

        [HttpPost("login-with-google")]
        public async Task<ActionResult<UserDto>> LoginWithGoogle([FromBody] ExternalAuthDto externalAuthDto)
        {
            var payload = await _tokenService.VerifyGoogleToken(externalAuthDto.Token);
            if (payload == null)
            {
                return BadRequest("Invalid External Authentication.");
            }

            // Kiểm tra user đã tồn tại chưa
            var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(payload.Email);

            if (user == null)
            {
                user = await _unitOfWork.AccountRepository.ExternalRegisterAsync(payload);
                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.TokenExpiryTime = DateTime.UtcNow.AddDays(7);

                // add new cart for user
                var userCart = new Cart
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id
                };
                await _unitOfWork.CartRepository.AddAsync(userCart);
                await _unitOfWork.SaveChangesAsync();
            }
            else if(user.Provider == payload.Issuer)
            {
                // Cập nhật refresh token cho user
                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.TokenExpiryTime = DateTime.UtcNow.AddDays(7);

                _unitOfWork.AccountRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                return BadRequest("This email has already been registered using another method!");
            }

            // Tạo access token và lưu vào cookie
            var claims = _tokenService.GenerateClaims(user);

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = user.RefreshToken;

            _tokenService.SetTokenInsideCookies(new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }, HttpContext);

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            HttpContext.Request.Cookies.TryGetValue("accessToken", out string? accessToken);
            HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken);

            if (accessToken == null || refreshToken == null) return BadRequest("Invalid client request!");

            try
            {
                var principal = _tokenService.GetPrincipalFromAccessToken(accessToken);

                var isPrincipalContainUserId = Guid.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);

                if (!isPrincipalContainUserId)
                {
                    _tokenService.RemoveTokenInsideCookies(HttpContext);
                    _cartService.RemoveCartIdInsideCookie(HttpContext);
                    return Unauthorized();
                }

                var user = await _unitOfWork.AccountRepository.FindAsync(userId);
                if (user == null
                    || user.RefreshToken != refreshToken
                    || user.TokenExpiryTime <= DateTime.UtcNow)
                {
                    //remove the cookie save token
                    _tokenService.RemoveTokenInsideCookies(HttpContext);
                    _cartService.RemoveCartIdInsideCookie(HttpContext);
                    return BadRequest("Invalid client request!");
                }

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                _unitOfWork.AccountRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _tokenService.SetTokenInsideCookies(new TokenDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }, HttpContext);

                return Ok();
            }
            catch (Exception ex)
            {
                _tokenService.RemoveTokenInsideCookies(HttpContext);
                _cartService.RemoveCartIdInsideCookie(HttpContext);
                return BadRequest("Invalid token!");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult> Revoke()
        {
            if(!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            var user = await _unitOfWork.AccountRepository.FindAsync(userId);
            if (user == null) return BadRequest();

            _unitOfWork.AccountRepository.RemoveUserToken(user);

            if(await _unitOfWork.SaveChangesAsync())
            {
                //remove cart id, refresh and access token cookie
                _tokenService.RemoveTokenInsideCookies(HttpContext);
                _cartService.RemoveCartIdInsideCookie(HttpContext);

                return NoContent();
            }
            
            return BadRequest();
        }

        [Authorize]
        [HttpPost("like-product/{id}")]
        public async Task<ActionResult> LikeProduct(string id)
        {
            if (!Guid.TryParse(id, out Guid productId)
                || !Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return BadRequest("Invalid client request!");

            var product = await _unitOfWork.ProductRepository.FindAsync(productId);
            if (product == null) return NotFound("Product is not existed!");

            if (await _unitOfWork.ProductRepository.IsProductLikedByCurrentUser(userId, productId))
                return Conflict("Product is liked by current user!");

            await _unitOfWork.AccountRepository.LikeProductAsync(userId, productId);

            if (await _unitOfWork.SaveChangesAsync())
                return Ok();

            return BadRequest("Failed to like product!");
        }

        [Authorize]
        [HttpDelete("unlike-product/{id}")]
        public async Task<ActionResult> UnlikeProduct(string id)
        {
            if (!Guid.TryParse(id, out Guid productId)
                || !Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return BadRequest("Invalid client request!");

            var product = await _unitOfWork.ProductRepository.FindAsync(productId);
            if (product == null) return NotFound("Product is not existed!");

            if (!await _unitOfWork.ProductRepository.IsProductLikedByCurrentUser(userId, productId))
                return Conflict("Product is not liked by current user!");

            await _unitOfWork.AccountRepository.UnlikeProductAsync(userId, productId);

            if (await _unitOfWork.SaveChangesAsync())
                return Ok();

            return BadRequest("Failed to unlike post!");
        }

        [HttpGet("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromQuery] string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_emailJwtSettings.Key);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out _);

                var email = principal.FindFirstValue(ClaimTypes.Email);

                if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                    return BadRequest("Invalid token!");

                var user = await _unitOfWork.AccountRepository.FindAsync(userId);
                if (user == null || user.Email != email)
                    return BadRequest("Người dùng không hợp lệ");

                if (user.IsAuthenticated)
                    return BadRequest("Email đã xác thực trước đó");

                user.IsAuthenticated = true;
                user.UpdateAt = DateTime.UtcNow;
                _unitOfWork.AccountRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Ok("Xác thực email thành công");
            }
            catch
            {
                return BadRequest("Token không hợp lệ hoặc đã hết hạn");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] EmailRequestDto dto)
        {
            var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return Ok(); // Trả về OK để tránh dò email

            await _emailSender.SendResetPasswordEmailAsync(user);

            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordDto dto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_emailJwtSettings.Key);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true
                }, out _);

                if (principal == null)
                    return BadRequest("Token không hợp lệ hoặc đã hết hạn");

                var email = principal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Email không tồn tại trong token");

                var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return BadRequest("Không tìm thấy người dùng");

                _unitOfWork.AccountRepository.ChangePassword(user, dto.NewPassword);

                if (await _unitOfWork.SaveChangesAsync())
                    return Ok("Đặt lại mật khẩu thành công");

                return BadRequest("Lỗi trong quá trình đặt lại mật khẩu");
            }
            catch
            {
                return BadRequest("Token không hợp lệ hoặc đã hết hạn");
            }

        }

        [HttpPost("send-verification-email")]
        public async Task<ActionResult> ResendVerificationEmail([FromBody] EmailRequestDto dto)
        {
            var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(dto.Email);

            if (user == null)
            {
                // Tránh tiết lộ thông tin user không tồn tại, trả về thông báo chung
                return Ok();
            }

            if (user.IsAuthenticated)
            {
                return BadRequest("Email này đã được xác thực rồi.");
            }

            await _emailSender.SendVerificationEmailAsync(user);

            return Ok("Một email xác thực mới đã được gửi.");
        }
    }
}
