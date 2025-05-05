using StackExchange.Redis;

namespace API.Extensions
{
    public static class RedisServiceExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration config)
        {
            var redisConnectionString = config.GetConnectionString("Redis")
                ?? throw new Exception("Cannot get redis connection string");
            services.AddSingleton<IConnectionMultiplexer>(opt =>
            {
                var configuration = ConfigurationOptions.Parse(redisConnectionString, true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            return services;
        }
    }
}
