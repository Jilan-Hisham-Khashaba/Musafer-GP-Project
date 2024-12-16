using AdminDashBoard.Models;
using AutoMapper;
using GP.Core.Entities;

namespace AdminDashBoard.Profiles
{
    public class MappingProfileee : Profile
    {
        public MappingProfileee()
        {
            CreateMap<CategoryModel, Category>().ForMember(pd => pd.TypeName, o => o.MapFrom(T => T.Name))
                .ReverseMap();

            CreateMap<CitiesModel, City>()
                .ForMember(pd => pd.Id, o => o.MapFrom(T => T.Id))
                 .ForMember(pd => pd.NameOfCity, o => o.MapFrom(T => T.cityName))
                      .ForMember(pd => pd.CountryId, o => o.MapFrom(T => T.countryId))
                       .ForMember(dest => dest.Country, opt => opt.MapFrom(src => new Country
                       {
                           NameCountry = src.countryName,
                           Contient = src.contient
                       }))
                .ReverseMap();


        }
    }
   
}
