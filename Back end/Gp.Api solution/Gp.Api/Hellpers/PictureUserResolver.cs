using AutoMapper;
using Gp.Api.Dtos;
using GP.APIs.Dtos;
using GP.core.Entities.identity;
using GP.Core.Entities;

namespace Gp.Api.Hellpers
{
    public class PictureUserResolver : IValueResolver<AppUser, PictureUserDto, string>
    {
        private readonly IConfiguration configuration;

        public PictureUserResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    

        public string Resolve(AppUser source, PictureUserDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PhotoPicture))
            {
                return $"{configuration["ApiBaseUrl"]}images/Pictures/{source.PhotoPicture}";
            }

            return string.Empty;
        }
    }
}
