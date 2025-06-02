using API.DTOs.SizeDtos;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class SizesController : BaseApiController
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSizes()
        {
            var sizes = await _sizeService.GetAllAsync();

            return Ok(sizes);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateSize(SizeDto dto)
        {
            var newSize = await _sizeService.CreateAsync(dto.Name);
            if (newSize != null)
                return Ok(newSize);

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSize(int id, SizeDto dto)
        {
            var updatedSize = await _sizeService.UpdateAsync(id, dto.Name);

            if (updatedSize != null)
            {
                return Ok(updatedSize);
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSize(int id)
        {
            var result = await _sizeService.DeleteAsync(id);

            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
