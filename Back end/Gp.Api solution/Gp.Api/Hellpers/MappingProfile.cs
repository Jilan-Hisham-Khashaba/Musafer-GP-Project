using AutoMapper;
using Gp.Api.Dtos;
using GP.APIs.Dtos;
using GP.core.Entities.identity;
using GP.Core.Entities;

using GP.Core.Repositories;
using GP.Repository.Data.Migrations;
using Microsoft.AspNetCore.Identity;
namespace Gp.Api.Hellpers
{
    public class MappingProfile:Profile
    {
        private readonly INameToIdResolver nameToIdResolver;
        public MappingProfile()
        {
           
            CreateMap<Trip, TripToDto>()
                 .ForMember(pd => pd.FromCityName, o => o.MapFrom(T => T.FromCity.NameOfCity))
                   .ForMember(pd => pd.ToCityName, o => o.MapFrom(T => T.ToCity.NameOfCity))
                   .ForMember(pd => pd.CountryIdFrom, o => o.MapFrom(T => T.FromCity.CountryId))
                    .ForMember(pd => pd.CountryIdTo, o => o.MapFrom(T => T.ToCity.CountryId))
                    .ForMember(pd => pd.CountryNameFrom, o => o.MapFrom(T => T.FromCity.Country.NameCountry))
            .ForMember(pd => pd.CountryNameTo, o => o.MapFrom(T => T.ToCity.Country.NameCountry))
                   //.ForMember(pd=>pd.CountryNameFrom,o=>o.MapFrom(T=>T.FromCity.CountryName))
                   //.ForMember(pd => pd.CountryNameTo, o => o.MapFrom(T => T.ToCity.CountryName))
                   //.ForMember(pd => pd.UserId, o => o.MapFrom(T => T.User.Id))
                   //.ForMember(pd => pd.UserName, o => o.MapFrom(T => T.User.DisplayName))
                .ReverseMap();



            CreateMap<Shipment, ShipmentToDto>()
               .ForMember(pd => pd.FromCityName, o => o.MapFrom(T => T.FromCity.NameOfCity))
    .ForMember(pd => pd.ToCityName, o => o.MapFrom(T => T.ToCity.NameOfCity))
    .ForMember(pd => pd.CountryIdFrom, o => o.MapFrom(T => T.FromCity.CountryId))
    .ForMember(pd => pd.CountryIdTo, o => o.MapFrom(T => T.ToCity.CountryId))
    .ForMember(pd => pd.CountryNameFrom, o => o.MapFrom(T => T.FromCity.Country.NameCountry))
    .ForMember(pd => pd.CountryNameTo, o => o.MapFrom(T => T.ToCity.Country.NameCountry))

    //.ForMember(pd => pd.CategoryId, o => o.MapFrom(T => T.Products.Select(p => p.CategoryId).FirstOrDefault()))
    //.ForMember(pd => pd.CategoryName, o => o.MapFrom(T => T.Products.Select(p => p.Category.TypeName).FirstOrDefault()))
    .ForMember(pd => pd.UserId, o => o.MapFrom(T => T.IdentityUserId))
  .ForMember(pd => pd.UserId, o => o.MapFrom(T => T.IdentityUserId))
  //.ForMember(pd=>pd.UserName,o=>o.MapFrom(T=>T.))
    .ReverseMap();

            CreateMap<Product, ProductsDto>()
                    .ForMember(pd => pd.ProductId, o => o.MapFrom(p => p.Id))
        .ForMember(pd => pd.ProductName, o => o.MapFrom(p => p.ProductName))
        .ForMember(pd => pd.ProductPrice, o => o.MapFrom(p => p.ProductPrice))
    .ForMember(pd => pd.ProductWeight, o => o.MapFrom(p => p.ProductWeight))
        .ForMember(pd => pd.ProductUrl, o => o.MapFrom(p => p.ProductUrl))
    .ForMember(pd => pd.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>())
    .ForMember(pd => pd.CategoryName, o => o.MapFrom(T => T.Category.TypeName))
    .ReverseMap();


            CreateMap<Category, CategoriesToDto>()
                .ForMember(c=>c.CategoryName, o => o.MapFrom(T => T.TypeName));

            CreateMap<City, CityToDto>()
             .ForMember(c => c.CityName, o => o.MapFrom(T => T.NameOfCity))
             .ForMember(c => c.CountryId, o => o.MapFrom(T => T.CountryId))
             .ForMember(c=>c.CountryName,o=>o.MapFrom(T=>T.Country.NameCountry));



           // CreateMap<createShipemt, Shipment>().ReverseMap();

            CreateMap<Comments, CommentDto>()
             .ForMember(c=>c.Content,o=>o.MapFrom(c=>c.Content))
             .ForMember(c=>c.Rate,o=>o.MapFrom(C=>C.Rate))
             .ForMember(pd => pd.UserId, o => o.MapFrom(T => T.UserId))
              .ForMember(pd => pd.UserIdSender, o => o.MapFrom(T => T.SenderId))
                .ReverseMap();

            CreateMap<Comments, CommentDTOFlask>()
            .ForMember(c => c.content, o => o.MapFrom(c => c.Content))
          
            .ForMember(pd => pd.UserId, o => o.MapFrom(T => T.UserId))
        
               .ReverseMap();

            CreateMap<RequestDto, Request>();

            CreateMap<VerificationDto, VerificationModel>();

            CreateMap<verficationFaccess, VrefactionFacesDto>().ForMember(pd => pd.image, o => o.MapFrom<ImageVerficationResolver>())
                .ReverseMap();

            CreateMap<GP.core.Entities.identity.Address, AddressDto>().ReverseMap();

            CreateMap<UserDto, AppUser>().ReverseMap();

            CreateMap<AddressDto, GP.Core.Entities.Orders.Address>();
        }



    }
}
