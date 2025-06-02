using API.Data.Entities;
using API.DTOs.ColorDtos;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ColorsController : BaseApiController
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<ActionResult> GetColors()
        {
            var colors = await _colorService.GetAllAsync();

            return Ok(colors);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateColor(ColorDto dto)
        {
            var newColor = await _colorService.CreateAsync(dto);
            if (newColor != null)
                return Ok(newColor);

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateColor(int id, ColorDto dto)
        {
            var updatedColor = await _colorService.UpdateAsync(id, dto);

            if (updatedColor != null)
            {
                return Ok(updatedColor);
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteColor(int id)
        {
            var result = await _colorService.DeleteAsync(id);

            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
