using API.Data.Entities;
using API.DTOs.UserDtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
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
                user = await _unitOfWork.AccountRepository.RegisterAsync(user, registerDto.Password);
                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.TokenExpiryTime = DateTime.UtcNow.AddDays(7);

                if(await _unitOfWork.SaveChangesAsync())
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
    }
}
