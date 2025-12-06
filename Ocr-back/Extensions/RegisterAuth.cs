using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocr_back.Config;
using Ocr_back.Data;
using Ocr_back.Models;
using Ocr_back.Services;

namespace Ocr_back.Extensions;

public static class RegisterAuth
{
    public static void AddAuthServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AuthDbContext>(op =>
        {
            op.UseNpgsql(config.GetConnectionString("AuthConnection"));
        });

        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AuthDbContext>();

        services.Configure<JwtOptions>(config.GetSection("Jwt"));

        var jwtOptions = config.GetSection("Jwt").Get<JwtOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions!.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

        services.AddScoped<TokenService>();
    }
}