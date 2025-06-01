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
        private readonly ISizeService _sizeService;
        private readonly IMapper _mapper;

        public SizesController(IUnitOfWork unitOfWork, ISizeService sizeService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _sizeService = sizeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSizes()
        {
            var sizes = await _sizeService.GetAllAsync();

            return Ok(sizes);
        }
    }
}
