using Gp.Api.Errors;
using Gp.Api.Hellpers;
using GP.Core.Repositories;
using GP.Core.Services;
using GP.Repository;
using GP.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Gp.Api.Extensions
{
    public static class ApplicationServicesExtension
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddScoped<ChatHub>();
            services.AddScoped<IChatHub, ChatHubAdapter>();
            // SignalR
            services.AddSignalR();

            services.AddScoped(typeof(IFaceComparisonResultRepository), typeof(FaceComparisonResultRepository));
            services.AddScoped(typeof(IGenericRepositroy<>), typeof(GenericRepositorty<>));
            services.AddScoped(typeof(FaceComparisonService));
            services.AddScoped(typeof(RecommendedService));
            services.AddScoped(typeof(IRequestRepository), typeof(RequestRepository));
         
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped(typeof(IGenericRepositroy<>), typeof(GenericRepositorty<>));
            services.AddScoped(typeof(IChatRepository), typeof(ChatRepository));
            services.AddScoped(typeof(INameToIdResolver), typeof(NameToIdResolver));
           services.AddScoped(typeof(ICountryRepository), typeof(CountryRepository));
            services.AddScoped(typeof(ICityRepository), typeof(CityRepository));
            services.AddScoped(typeof(ICategoryRepository), typeof(categoryRepository));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));



            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                    .SelectMany(P => P.Value.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray();


                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });
        
            return services;    
        }
    }
}
