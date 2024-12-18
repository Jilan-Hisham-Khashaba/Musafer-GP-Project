using AutoMapper;
using Gp.Api.Dtos;
using Gp.Api.Errors;
using GP.Core.Entities.Orders;
using GP.Core.Repositories;
using GP.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Gp.Api.Hellpers;
using GP.Core.Entities;
using Microsoft.AspNetCore.Identity;
using GP.core.Entities.identity;
using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using Twilio.Http;

namespace Gp.Api.Controllers
{
    [Authorize]
    public class OrdersController : ApiBaseController
    {
        private readonly IGenericRepositroy<Order> orderRepo;
        private readonly IGenericRepositroy<Trip> tripRepo;
        private readonly IConfiguration configuration1;
        private readonly UserManager<AppUser> userManager;
        private readonly IOrderService orderService;
        private readonly IMapper mapper;
        private readonly ProductPictureUrlResolver productPictureUrlResolver;
        public OrdersController(IGenericRepositroy<Order> orderRepo, IGenericRepositroy<Trip> TripRepo, IConfiguration configuration,IConfiguration configuration1 ,UserManager<AppUser> userManager, IOrderService orderService,IMapper mapper)
        {
            this.orderRepo = orderRepo;
            tripRepo = TripRepo;
            this.configuration1 = configuration1;
            this.userManager = userManager;
            this.orderService = orderService;
            this.productPictureUrlResolver = new ProductPictureUrlResolver(configuration);
            this.mapper = mapper;
        }
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

        [HttpPost] //POST:API/Orders
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var useremaill = await userManager.FindByEmailAsync(Email);
            var address = mapper.Map<AddressDto, GP.Core.Entities.Orders.Address>(orderDto.ShippingAddress);

            var Order = await orderService.CreateOrderAsync(useremaill.Id, orderDto.RequestId, address);

            if (Order is null) return BadRequest(new ApiResponse(400));

          
            return Ok(Order);
        }


        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound )]
        [HttpGet] //Get:/api/orders
 
        public async Task<IActionResult> GetOrdersForUser()
        {
            var EmailTrip = User.FindFirstValue(ClaimTypes.Email);
            var useremaill = await userManager.FindByEmailAsync(EmailTrip);
            var idds = useremaill.Id;
            var orders = await orderService.GetOrdersForUserAsync(idds);
            
            var orderDtos = new List<OrderDto>();

           foreach (var order in orders)
                {
                var recieved = await userManager.FindByIdAsync(order.recievedUserId);
                if (order.Email == EmailTrip || order.recievedUserId == recieved.Id)
                {
                    var userRequest = await userManager.FindByIdAsync(order.Request.UserId);
                    var userTrip = await userManager.FindByIdAsync(order.Request.Trip.UserId);
                    var user = await userManager.FindByIdAsync(order.Request.Shipment.IdentityUserId);
                    var orderDto = new OrderDto
                    {
                        id = order.Id,
                        RecievedUserId = order.recievedUserId,
                        RequestId = order.RequestId,
                        Status = (int)order.Status,
                        email = order.Email,
                        Request = new RequestDto
                        {
                            ISConvertedToOrder=order.Request.IsConvertedToOrder,
                            Id = order.RequestId,
                            ShipmentToDto = new ShipmentToDto
                            {
                                Id = order.Request.Shipment.Id, // تأكد من تعريف sh
                                Weight = order.Request.Shipment.Weight, // تأكد من تعريف shipment
                                FromCityID = order.Request.Shipment.FromCityID,
                                FromCityName = order.Request.Shipment.FromCity.NameOfCity,
                                CountryIdFrom = order.Request.Shipment.FromCity.Country.Id,
                                CountryNameFrom = order.Request.Shipment.FromCity.Country.NameCountry,
                                ToCityId = order.Request.Shipment.ToCityId,
                                ToCityName = order.Request.Shipment.ToCity.NameOfCity,
                                CountryIdTo = order.Request.Shipment.ToCity.Country.Id,
                                CountryNameTo = order.Request.Shipment.ToCity.Country.NameCountry,
                                DateOfRecieving = order.Request.Shipment.DateOfRecieving,
                                Address = order.Request.Shipment.Address,
                                UserName = user.UserName,
                                UserId = order.Request.Shipment.IdentityUserId,
                                UserPicture = new PictureUserResolver(configuration1).Resolve(user, null, null, null),
                                // UserName = .UserName, // تأكد من تعريف user
                                Products = order.Request.Shipment.Products.Select(product => new ProductsDto
                                {
                                    ProductId = product.Id,
                                    ProductName = product.ProductName,
                                    ProductPrice = product.ProductPrice,
                                    ProductWeight = product.ProductWeight,
                                    ProductUrl = product.ProductUrl,
                                    PictureUrl = productPictureUrlResolver.Resolve(product, null, null, null),
                                }).ToList()
                            },
                            TripToDto = new TripToDto
                            {
                                Id = order.Request.Trip.Id,
                                FromCityID = order.Request.Trip.FromCityID,
                                FromCityName = order.Request.Trip.FromCity.NameOfCity,
                                CountryIdFrom = order.Request.Trip.FromCity.Country.Id,
                                CountryNameFrom = order.Request.Trip.FromCity.Country.NameCountry,
                                availableKg = order.Request.Trip.availableKg,
                                ToCityId = order.Request.Trip.ToCity.Id,
                                ToCityName = order.Request.Trip.ToCity.NameOfCity,
                                CountryIdTo = order.Request.Trip.ToCity.Country.Id,
                                CountryNameTo = order.Request.Trip.ToCity.Country.NameCountry,
                                arrivalTime = order.Request.Trip.arrivalTime,
                                dateofCreation = order.Request.Trip.DateofCreation,
                                UserId = order.Request.Trip.UserId,
                                UserName = userTrip.UserName,
                                UnitPricePerKg = order.Request.Trip.UnitPricePerKg,
                                SubTotal = order.Request.Trip.SubTotal,
                                UserPhoto = new PictureUserResolver(configuration1).Resolve(userTrip, null, null, null),
                            },
                            RequestUserId = order.Request.UserId,


                        },


                    };


                    orderDtos.Add(orderDto);
                }
            }

            return Ok(orderDtos);


           



        }

        [HttpGet("{id}")] //get:api/orders/1
        public async Task<ActionResult<OrderDto>>GetOrderForUser(int id)
        {
            var Email=User.FindFirstValue(ClaimTypes.Email);
            var useremaill = await userManager.FindByEmailAsync(Email);
            var Order = await orderService.GetOrderByIdAsync(id);
            var orderDtos = new List<OrderDto>();

          
                var userRequest = await userManager.FindByIdAsync(Order.Request.UserId);
                var userTrip = await userManager.FindByIdAsync(Order.Request.Trip.UserId);
                var user = await userManager.FindByIdAsync(Order.Request.Shipment.IdentityUserId);
                var orderDto = new OrderDto
                {
                    RecievedUserId = Order.recievedUserId,
                    RequestId = Order.RequestId,
                    Status = (int)Order.Status,
                    email = Order.Email,
                    Request = new RequestDto
                    {
                        ISConvertedToOrder = Order.Request.IsConvertedToOrder,
                        Id = Order.RequestId,
                        ShipmentToDto = new ShipmentToDto
                        {
                            Id = Order.Request.Shipment.Id, // تأكد من تعريف sh
                            Weight = Order.Request.Shipment.Weight, // تأكد من تعريف shipment
                            FromCityID = Order.Request.Shipment.FromCityID,
                            FromCityName = Order.Request.Shipment.FromCity.NameOfCity,
                            CountryIdFrom = Order.Request.Shipment.FromCity.Country.Id,
                            CountryNameFrom = Order.Request.Shipment.FromCity.Country.NameCountry,
                            ToCityId = Order.Request.Shipment.ToCityId,
                            ToCityName = Order.Request.Shipment.ToCity.NameOfCity,
                            CountryIdTo = Order.Request.Shipment.ToCity.Country.Id,
                            CountryNameTo = Order.Request.Shipment.ToCity.Country.NameCountry,
                            DateOfRecieving = Order.Request.Shipment.DateOfRecieving,
                            Address = Order.Request.Shipment.Address,
                            UserName = user.UserName,
                            UserId = Order.Request.Shipment.IdentityUserId,
                            UserPicture= new PictureUserResolver(configuration1).Resolve(user, null, null, null),
            // UserName = .UserName, // تأكد من تعريف user
            Products = Order.Request.Shipment.Products.Select(product => new ProductsDto
                            {
                                ProductId = product.Id,
                                ProductName = product.ProductName,
                                ProductPrice = product.ProductPrice,
                                ProductWeight = product.ProductWeight,
                                ProductUrl = product.ProductUrl,
                                PictureUrl = productPictureUrlResolver.Resolve(product, null, null, null),
                            }).ToList()
                        },
                        TripToDto = new TripToDto
                        {
                            Id = Order.Request.Trip.Id,
                            FromCityID = Order.Request.Trip.FromCityID,
                            FromCityName = Order.Request.Trip.FromCity.NameOfCity,
                            CountryIdFrom = Order.Request.Trip.FromCity.Country.Id,
                            CountryNameFrom = Order.Request.Trip.FromCity.Country.NameCountry,
                            availableKg = Order.Request.Trip.availableKg,
                            ToCityId = Order.Request.Trip.ToCity.Id,
                            ToCityName = Order.Request.Trip.ToCity.NameOfCity,
                            CountryIdTo = Order.Request.Trip.ToCity.Country.Id,
                            CountryNameTo = Order.Request.Trip.ToCity.Country.NameCountry,
                            arrivalTime = Order.Request.Trip.arrivalTime,
                            dateofCreation = Order.Request.Trip.DateofCreation,
                            UserId = Order.Request.Trip.UserId,
                            UserName = userTrip.UserName,
                            UnitPricePerKg = Order.Request.Trip.UnitPricePerKg,
                            SubTotal = Order.Request.Trip.SubTotal,
                            UserPhoto = new PictureUserResolver(configuration1).Resolve(userTrip, null, null, null),
                        },
                        RequestUserId = Order.Request.UserId,


                    },

                };


                orderDtos.Add(orderDto);

            if (Order is null) return NotFound(new ApiResponse(404));
            return Ok(orderDtos);
      
          
        }

    }

}
