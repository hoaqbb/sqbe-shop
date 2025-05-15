using API.Data.Entities;
using API.Helpers;
using API.Interfaces;
using API.Repositories;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<EcommerceDbContext>(opt =>
                opt.UseNpgsql(config.GetConnectionString("PostgresDb"))
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IUserService, UserService>();

            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.Configure<VnPaySettings>(config.GetSection("VNPaySettings"));

            return services;
        }
    }
}
