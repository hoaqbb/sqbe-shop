using API.Data.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PromotionsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionService _promotionService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public PromotionsController(IUnitOfWork unitOfWork, IPromotionService promotionService, IMapper mapper, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _promotionService = promotionService;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpGet("{code}/apply")]
        public async Task<IActionResult> ValidatePromotion(string code, [FromQuery] decimal subtotal)
        {
            var userId = HttpContext.GetUserIdFromTokenInsideCookie(_tokenService);
            var result = await _promotionService.ApplyPromotionAsync(code, subtotal, userId);

            if (!result.IsValid)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
