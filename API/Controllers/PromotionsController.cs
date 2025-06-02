using API.Data.Entities;
using API.DTOs.PromotionDtos;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PromotionsController : BaseApiController
    {
        private readonly IPromotionService _promotionService;
        private readonly ITokenService _tokenService;

        public PromotionsController(IPromotionService promotionService, ITokenService tokenService)
        {
            _promotionService = promotionService;
            _tokenService = tokenService;
        }

        [HttpGet("{code}/apply")]
        public async Task<ActionResult> ValidatePromotion(string code, [FromQuery] decimal subtotal)
        {
            var userId = HttpContext.GetUserIdFromTokenInsideCookie(_tokenService);
            var result = await _promotionService.ApplyPromotionAsync(code, subtotal, userId);

            if (!result.IsValid)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreatePromotion(CreatePromotionDto dto)
        {
            var result = await _promotionService.CreateAsync(dto);
            if(result.IsValid)
            {
                return Ok(result.Promotion);
            }

            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePromotion(int id, [FromBody] UpdatePromotionDto dto)
        {
            var result = await _promotionService.UpdateAsync(id, dto);
            if(result.IsValid)
            {
                return Ok(result.Promotion);
            }

            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePromotionById(int id)
        {
            var result = await _promotionService.DeteleAsync(id);

            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
