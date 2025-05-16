using API.Data.Entities;
using API.DTOs.SizeDtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class SizesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SizesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSizes()
        {
            var sizes = await _unitOfWork.Repository<Size>()
                .GetAllProjectedAsync<SizeDto>(_mapper.ConfigurationProvider);

            return Ok(sizes);
        }
    }
}
