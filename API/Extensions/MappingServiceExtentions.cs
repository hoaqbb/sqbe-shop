using API.Helpers.MappingProfiles;
using System.Reflection;

namespace API.Extensions
{
    public static class MappingServiceExtentions
    {
        public static IServiceCollection AddMappingService(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Program).Assembly);
            return services;
        }
    }
}
