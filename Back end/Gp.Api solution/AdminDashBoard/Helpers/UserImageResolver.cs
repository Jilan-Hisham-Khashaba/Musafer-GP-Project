using AdminDashBoard.Models;
using AutoMapper;
using Gp.Api.Dtos;
using GP.core.Entities.identity;

namespace AdminDashBoard.Helpers
{
    public class UserImageResolver : IValueResolver<AppUser, UserView, string>
    {
        private readonly IConfiguration configuration;

        public UserImageResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public string Resolve(AppUser source, UserView destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PhotoPicture))
            {
                return $"{configuration["ApiBaseUrl"]}images/PictureAdmin/{source.PhotoPicture}";
            }

            return string.Empty;
        }
    }
}
