using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["SecretKey"]))
                };

                opts.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        ctx.Request.Cookies.TryGetValue("accessToken", out string? accessToken);
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();
                            try
                            {
                                var token = tokenHandler.ReadJwtToken(accessToken);
                                if (token.ValidTo > DateTime.UtcNow)
                                {
                                    // Token is expired, don't set ctx.Token
                                    // This will cause authentication to fail with 401 Unauthorized
                                    ctx.Token = accessToken;
                                }
                            }
                            catch
                            {
                                ctx.NoResult();
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
