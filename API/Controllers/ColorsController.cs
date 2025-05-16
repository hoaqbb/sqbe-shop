using API.Data.Entities;
using API.DTOs.ColorDtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ColorsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ColorsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetColors()
        {
            var colors = await _unitOfWork.Repository<Color>()
                .GetAllProjectedAsync<ColorDto>(_mapper.ConfigurationProvider);

            return Ok(colors);
        }
    }
}
