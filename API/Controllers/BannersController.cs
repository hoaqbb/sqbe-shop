using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BannersController : BaseApiController
    {
        private readonly IBannerService _bannerService;

        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        public async Task<ActionResult> GetBanners()
        {
            var banners = await _bannerService.GetBannersAsync();

            return Ok(banners);
        }
    }
}
