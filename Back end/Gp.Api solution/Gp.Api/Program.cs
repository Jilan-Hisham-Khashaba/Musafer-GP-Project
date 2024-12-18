using Gp.Api.Extensions;
using Gp.Api.Hellpers;
using Gp.Api.Middlewares;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Repository;
using GP.Repository.Data;
using GP.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using GP.APIs.Exentations;
using GP.core.Entities.identity;
using  GP.Repository.Identity;
using Gp.Api.Twilio;
using Microsoft.Extensions.Caching.Memory;
using GP.Services;

namespace Gp.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

           builder.Services.AddControllers();

            //builder.Services.AddControllers()
            //      .AddJsonOptions(options =>
            //          {
            //              options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            //              options.JsonSerializerOptions.WriteIndented = true;
            //          });


            ////////////////////
            ///*------Database
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //-------------------------Extentions Services
            //----------------------
            builder.Services.AddApplicationServices();
      
                            // باقي الإعدادات الأخرى
            

            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddSwaggerServices();
            ////twilio
            builder.Services.Configure<TwilioSetting>(builder.Configuration.GetSection("Twilio"));
            builder.Services.AddTransient<ISmsMessage, SmsSetting>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .WithOrigins(builder.Configuration["FrontUrl"]);
                });
            });

            var app = builder.Build();
            //Explicity
            var scope = app.Services.CreateScope();   //services scopped
            var Services = scope.ServiceProvider; //di
            //loggerFacotry
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                var dbContext = Services.GetRequiredService<StoreContext>(); //ask clr to create object from store context explicity
                await dbContext.Database.MigrateAsync(); //update-datebase
                await StoreContextSeed.SeedAsync(dbContext);
                var identityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await identityDbContext.Database.MigrateAsync(); //update-database

                var userManger = Services.GetRequiredService<UserManager<AppUser>>();
                await AddIdentityDbContextSeed.SeedUserAsync(userManger);
            }
            catch (Exception ex)
            {
                var logger = LoggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "an error Occured during applay Migration");

            }
            #region Configure Kestrel Middlewares
            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            app.UseRouting();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }

            app.UseSwagger();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/hub");
                endpoints.MapControllers();
            });

            #endregion

            app.Run();
        }
    }
}
