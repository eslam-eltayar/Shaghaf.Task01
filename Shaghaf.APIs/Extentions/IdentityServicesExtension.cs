using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shaghaf.Core.Entities.Identity;
using Shaghaf.Core.Services;
using Shaghaf.Repository.Identity;
using Shaghaf.Service.TokenService;
using System.Text;

namespace Shaghaf.APIs.Extentions
{
    public static class IdentityServicesExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<ITokenService, TokenService>();

            Services.AddIdentity<AppUser, IdentityRole>()
                           .AddEntityFrameworkStores<AppIdentityDbContext>();

            Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)// UserManager / SignIn Manager / RoleManager
                                      .AddJwtBearer(options =>
                                      {
                                          options.TokenValidationParameters = new TokenValidationParameters()
                                          {
                                              ValidateIssuer = true,
                                              ValidIssuer = configuration["JWT:ValidIssuer"],
                                              ValidateAudience = true,
                                              ValidAudience = configuration["JWT:ValidAudience"],
                                              ValidateIssuerSigningKey = true,
                                              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"] ?? string.Empty)),
                                              ValidateLifetime = true,

                                          };
                                      });

            Services.AddIdentityCore<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);
            return Services;
        }
    }
}
