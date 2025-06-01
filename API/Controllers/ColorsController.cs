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

        public ColorsController(IUnitOfWork unitOfWork, IColorService colorService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _colorService = colorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetColors()
        {
            var colors = await _colorService.GetAllAsync();

            return Ok(colors);
        }
    }
}
