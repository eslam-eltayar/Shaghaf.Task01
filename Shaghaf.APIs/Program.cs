
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shaghaf.APIs.Extentions;
using Shaghaf.APIs.Middlewares;
using Shaghaf.Core.Entities.Identity;
using Shaghaf.Core.Services;
using Shaghaf.Repository.Identity;
using Shaghaf.Service.SMSService;

namespace Shaghaf.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.


            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
            builder.Services.AddTransient<ISMSService, SMSService>();
            #endregion


            var app = builder.Build();

            #region Update - Database & applying all pending Migrations

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            // Ask CLR for Creatig Object from DbContext Explicitly

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();

            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            try
            {

                await _identityDbContext.Database.MigrateAsync();

                await AppIdentityDbContextSeed.SeedUserAsync(userManager);

            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex);
                logger.LogError(ex, "An Error Has Been occured during apply the Migration");
            }

            #endregion

            #region Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            #endregion

            app.MapControllers();

            app.Run();
        }
    }
}
