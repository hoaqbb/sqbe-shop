using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected async Task<PaginatedResult<T>> CreatePaginatedResult<T>(
            IGenericRepository<T> repo,
            ISpecification<T> spec, 
            int pageIndex, 
            int pageSize
            ) where T : class
        {
            var items = await repo.ListAsync(spec);
            var count = await repo.CountAsync(spec);

            var pagination = new PaginatedResult<T>(items, count, pageIndex, pageSize);

            return pagination;
        }

        protected async Task<PaginatedResult<TResult>> CreatePaginatedResult<T, TResult>(
            IGenericRepository<T> repo,
            ISpecification<T> spec, 
            int pageIndex, 
            int pageSize, 
            AutoMapper.IConfigurationProvider config
            ) where T : class
        {
            var items = await repo.ListAsync<TResult>(spec, config);
            var count = await repo.CountAsync(spec);

            var pagination = new PaginatedResult<TResult>(items, count, pageIndex, pageSize);

            return pagination;
        }
    }
}
