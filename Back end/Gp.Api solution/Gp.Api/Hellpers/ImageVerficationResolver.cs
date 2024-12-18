using AutoMapper;
using Gp.Api.Dtos;
using GP.Core.Entities;

namespace Gp.Api.Hellpers
{
    public class ImageVerficationResolver : IValueResolver<verficationFaccess, VrefactionFacesDto, string>
    {
        private readonly IConfiguration configuration;

        public ImageVerficationResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(verficationFaccess source, VrefactionFacesDto destination, string destMember, ResolutionContext context)
        {
           
                if (!string.IsNullOrEmpty(source.ImageName))
                {
                    return $"{configuration["ApiBaseUrl"]}images/flaskApi/{source.ImageName}";
                }
            
            return string.Empty;

        }

        public string? Resolve(FaceComparison faceComparisonResult, object value1, object value2, object value3)
        {

            if (!string.IsNullOrEmpty(faceComparisonResult.ImageName))
            {
                return $"{configuration["ApiBaseUrl"]}images/flaskApi/{faceComparisonResult.ImageName}";
            }

            return string.Empty;
        }
    }
}
