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
    }
}
