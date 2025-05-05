using API.Data.Entities;
using API.DTOs.UserDtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        public AccountsController(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper,
            ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
            _cartService = cartService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(registerDto.Email);
            if (user != null)
            {
                return BadRequest("User is existed!");
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
                    var claims = _tokenService.GenerateClaims(user);
                    var accessToken = _tokenService.GenerateAccessToken(claims);

                    _tokenService.SetTokenInsideCookies(new TokenDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = user.RefreshToken
                    },
                        HttpContext);
                    return Ok(_mapper.Map<UserDto>(user));
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

            if (user is null) return BadRequest("Invalid email or password!");

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

            return BadRequest("Failed to like post!");
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
    }
}
