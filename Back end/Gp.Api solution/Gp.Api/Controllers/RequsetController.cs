using AutoMapper;
using Gp.Api.Dtos;
using Gp.Api.Errors;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Gp.Api.Hellpers;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Twilio.Http;
using GP.Services;
using Microsoft.AspNetCore.SignalR;
using GP.Core.Services;
using Emgu.CV.Ocl;
using GP.Repository.Data;

namespace Gp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequsetController : ApiBaseController
    {
        private readonly IConfiguration configuration;
        private readonly StoreContext context;
        private readonly IChatHub chatHub1;
        private readonly IHubContext<ChatHub> chatHub;
        private readonly IRequestRepository requestRepository;
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly IGenericRepositroy<Trip> tripRepo;

        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        private readonly ProductPictureUrlResolver productPictureUrlResolver;
        public RequsetController(IConfiguration configuration,StoreContext context ,IChatHub _chatHub, IHubContext<ChatHub> chatHub, IRequestRepository requestRepository, IGenericRepositroy<Shipment> ShipmentRepo, IGenericRepositroy<Trip> tripRepo, IMapper mapper, UserManager<AppUser> userManager)
        {
            this.configuration = configuration;
            this.context = context;
            chatHub1 = _chatHub;
            this.chatHub = chatHub;
            this.requestRepository = requestRepository;
            shipmentRepo = ShipmentRepo;
            this.tripRepo = tripRepo;

            this.mapper = mapper;
            this.userManager = userManager;
            this.productPictureUrlResolver = new ProductPictureUrlResolver(configuration);
        }

        [HttpGet]

        public async Task<ActionResult<RequestDto>> GetRequestShipemtWTrip(int Id)
        {
            var request = await requestRepository.GetRequestAsync(Id);

            if (request == null)
            {
                // إذا كان الطلب غير موجود، يمكنك إنشاء كائن Request جديد
                // ولكن يجب استخدامه أو تخزينه في قاعدة البيانات حسب المتطلبات
                request = new GP.Core.Entities.Request(Id);
            }

            // جلب بيانات الشحنة
            var specShipment = new ShipmentSpecification(request.ShipmentId);
            var shipment = await shipmentRepo.GetByIdwithSpecAsyn(specShipment);

            // جلب بيانات الرحلة
            var specTrip = new TripSpecifications(request.TripId);
            var trip = await tripRepo.GetByIdwithSpecAsyn(specTrip);

            // تحديث كائن الطلب ليتضمن بيانات الشحنة والرحلة
            request.Shipment = shipment;
            request.Trip = trip;

            // إنشاء كائن RequestDto من بيانات الطلب والشحنة والرحلة
            var user = await userManager.FindByIdAsync(shipment.IdentityUserId);
            var userTrip = await userManager.FindByIdAsync(trip.UserId);
            var requestDto = new RequestDto
            {
                ISConvertedToOrder=request.IsConvertedToOrder,
                Id = Id,
                ShipmentToDto = new ShipmentToDto
                {
                    Id = shipment.Id,
                    Weight = shipment.Weight,
                    FromCityID = shipment.FromCityID,
                    FromCityName = shipment.FromCity.NameOfCity,
                    CountryIdFrom = shipment.FromCity.Country.Id,
                    CountryNameFrom = shipment.FromCity.Country.NameCountry,
                    ToCityId = shipment.ToCityId,
                    ToCityName = shipment.ToCity.NameOfCity,
                    CountryIdTo = shipment.ToCity.Country.Id,
                    CountryNameTo = shipment.ToCity.Country.NameCountry,
                    DateOfRecieving = shipment.DateOfRecieving,
                    Address = shipment.Address,
                    UserId = shipment.IdentityUserId,
                    UserName = user.UserName,
                    UserPicture= new PictureUserResolver(configuration).Resolve(user, null, null, null),
                    // استخدام LINQ للوصول إلى تفاصيل المنتجات المطلوبة
                    Products = shipment.Products.Select(product => new ProductsDto
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        ProductPrice = product.ProductPrice,
                        ProductWeight = product.ProductWeight,
                        ProductUrl = product.ProductUrl,
                        PictureUrl = productPictureUrlResolver.Resolve(product, null, null, null),

                        //CategoryId = product.Category.Id,
                        //CategoryName = product.Category.TypeName
                    }).ToList()
                },
                TripToDto = new TripToDto
                {
                    Id = trip.Id,
                    FromCityID = trip.FromCityID,
                    FromCityName = trip.FromCity.NameOfCity,
                    CountryIdFrom = trip.FromCity.Country.Id,
                    CountryNameFrom = trip.FromCity.Country.NameCountry,
                    availableKg = trip.availableKg,
                    ToCityId = trip.ToCity.Id,
                    ToCityName = trip.ToCity.NameOfCity,
                    CountryIdTo = trip.ToCity.Country.Id,
                    CountryNameTo = trip.ToCity.Country.NameCountry,
                    arrivalTime = trip.arrivalTime,
                    dateofCreation = trip.DateofCreation,
                    UserId = trip.UserId,
                    UserName = userTrip.UserName,
                    UnitPricePerKg = trip.UnitPricePerKg,
                    SubTotal = trip.SubTotal,
                    UserPhoto = new PictureUserResolver(configuration).Resolve(userTrip, null, null, null),
                },
              
                RequestUserId = request.UserId,
                SenderUserId = request.SenderId
            };

            return Ok(requestDto); // أو يمكنك استخدام Ok إذا كان الطلب ناجحًا
        }
        [Authorize]
        [HttpGet("ALLRequest")]
       
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetAllRequests()
        {
            // جلب جميع الطلبات من قاعدة البيانات
            var allRequests = await requestRepository.GetAllRequestsAsync();

            // إنشاء قائمة لتخزين بيانات جميع الطلبات
            var requestDtos = new List<RequestDto>();
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);
            // حلق عبر كل طلب واستخراج بيانات الشحنة والرحلة وإضافتها إلى القائمة
            foreach (var request in allRequests)
            {

                var user = await userManager.FindByIdAsync(request.Shipment.IdentityUserId);
                var userTrip = await userManager.FindByIdAsync(request.Trip.UserId);
                if (existingUser.Id == user.Id || existingUser.Id == userTrip.Id)
                {
                    var specShipment = new ShipmentSpecification(request.ShipmentId);
                    var shipment = await shipmentRepo.GetByIdwithSpecAsyn(specShipment);

                    var specTrip = new TripSpecifications(request.TripId);
                    var trip = await tripRepo.GetByIdwithSpecAsyn(specTrip);


                    var requestDto = new RequestDto
                    {
                        ISConvertedToOrder = request.IsConvertedToOrder,
                        Id = request.RequestId,
                        ShipmentToDto = new ShipmentToDto
                        {
                            Id = shipment.Id,
                            Weight = shipment.Weight,
                            FromCityID = shipment.FromCityID,
                            FromCityName = shipment.FromCity.NameOfCity,
                            CountryIdFrom = shipment.FromCity.Country.Id,
                            CountryNameFrom = shipment.FromCity.Country.NameCountry,
                            ToCityId = shipment.ToCityId,
                            ToCityName = shipment.ToCity.NameOfCity,
                            CountryIdTo = shipment.ToCity.Country.Id,
                            CountryNameTo = shipment.ToCity.Country.NameCountry,
                            DateOfRecieving = shipment.DateOfRecieving,
                            Address = shipment.Address,
                            UserId = shipment.IdentityUserId,
                            UserName = user.UserName,
                            UserPicture = new PictureUserResolver(configuration).Resolve(user, null, null, null),
                            Products = shipment.Products.Select(product => new ProductsDto
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
                            Id = trip.Id,
                            FromCityID = trip.FromCityID,
                            FromCityName = trip.FromCity.NameOfCity,
                            CountryIdFrom = trip.FromCity.Country.Id,
                            CountryNameFrom = trip.FromCity.Country.NameCountry,
                            ToCityId = trip.ToCity.Id,
                            ToCityName = trip.ToCity.NameOfCity,
                            CountryIdTo = trip.ToCity.Country.Id,
                            CountryNameTo = trip.ToCity.Country.NameCountry,
                            availableKg = trip.availableKg,
                            arrivalTime = trip.arrivalTime,
                            dateofCreation = trip.DateofCreation,
                            UserId = trip.UserId,
                            UserName = userTrip.UserName,
                            UnitPricePerKg = trip.UnitPricePerKg,
                            SubTotal = trip.SubTotal,
                            UserPhoto = new PictureUserResolver(configuration).Resolve(userTrip, null, null, null),
                        },
                        RequestUserId = request.UserId,
                        SenderUserId =request.SenderId // إضافة `senderUserId` هنا
                    };

                    requestDtos.Add(requestDto);
                }
            }
            return Ok(requestDtos); 
        }


        [Authorize]
        [HttpPost] //post:api/request
        public async Task<ActionResult<GP.Core.Entities.Request>> UpdateRequest(RequestDto request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);
            if (request.ShipmentToDto.UserId == existingUser.Id || request.TripToDto.UserId == existingUser.Id || request.TripToDto.FromCityID == request.ShipmentToDto.FromCityID)
            {
                if (request.ShipmentToDto.UserId != request.TripToDto.UserId)
                {
                    var senderUserId = request.TripToDto.UserId != existingUser.Id ? request.TripToDto.UserId : request.ShipmentToDto.UserId;
                    var mappedRequest = mapper.Map<RequestDto, GP.Core.Entities.Request>(request);
                    mappedRequest.UserId = existingUser?.Id;
                    mappedRequest.SenderId = senderUserId;
                    mappedRequest.TripId = request.TripToDto.Id;
                    mappedRequest.ShipmentId = request.ShipmentToDto.Id;
                    var CreatedOrUpdatedRequest = await requestRepository.UpdateRequestAsync(mappedRequest);

                    if (CreatedOrUpdatedRequest.Shipment.FromCityID == CreatedOrUpdatedRequest.Trip.FromCityID || CreatedOrUpdatedRequest.Shipment.ToCityId == CreatedOrUpdatedRequest.Trip.ToCityId)
                    {
                        await requestRepository.SaveChangesAsync();

                        if (CreatedOrUpdatedRequest is null) return BadRequest(new ApiResponse(400));

                        var notificationMessage = $"You have a new request from {existingUser.Email}";
                        await chatHub1.SendNotification(senderUserId, notificationMessage);

                        return Ok(new { CreatedOrUpdatedRequest, message = "Request is created" });
                    }
                    else
                    {
                        return BadRequest(new { message = "oopps not add request same from country" });
                    }
                }
                else
                {
                    return BadRequest(new ApiResponse(400, "Cannot create request for same user's trip or shipment."));
                }
            }
            else
            {
                return BadRequest(new ApiResponse(400, "Cannot create request for another user's trip or shipment."));
            }
        }
        [Authorize]
        [HttpDelete] //delete:api/request
        public async Task<ActionResult<bool>> DeleteRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound(new { message = "Request not found" });
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);
            if (request.UserId != existingUser.Id && request.SenderId != existingUser.Id)
            {
                return BadRequest(new { message = "You are not authorized to delete this request" });
            }
            var isDeleted = await requestRepository.DeleteRequestAsync(id);

            if (!isDeleted)
            {
                return BadRequest(new { message = "Failed to delete the request" });
            }
            var notificationMessage = $"Your request has been rejected by {existingUser.Email}";
            await chatHub1.SendNotification(request.UserId, notificationMessage);

            return Ok(new { isDeleted, message = "Request is deleted" });
        }





    }
}
