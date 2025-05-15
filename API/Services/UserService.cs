using API.Data.Entities;
using API.DTOs.UserDtos;
using API.Interfaces;
using AutoMapper;
using System.Security.Claims;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserInfoAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var id = httpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserDto? user = null;

            if(Guid.TryParse(id, out Guid userId))
            {
                user = await _unitOfWork.Repository<User>()
                    .GetSingleProjectedAsync<UserDto>(
                        x => x.Id == userId, 
                        _mapper.ConfigurationProvider);
            }
            return user;
        }
    }
}
