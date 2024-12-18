using AutoMapper;
using Gp.Api.Dtos;
using Gp.Api.Errors;
using Gp.Api.Hellpers;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using GP.Repository;
using GP.Repository.Data.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GP.core.Entities.identity;
using GP.core.Sepecifitction;
using Microsoft.Extensions.Configuration;

namespace Gp.Api.Controllers
{

    public class ShipmentsController : ApiBaseController
    {
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ICountryRepository countryRepository;
        private readonly ICityRepository cityRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IGenericRepositroy<Product> productRepo;
        private readonly UserManager<AppUser> userManager;

        public ShipmentsController(IGenericRepositroy<Shipment> ShipmentRepo,  IConfiguration configuration ,IMapper mapper, ICountryRepository countryRepository, ICityRepository cityRepository, ICategoryRepository categoryRepository, IGenericRepositroy<Product> productRepo, UserManager<AppUser> userManager)
        {
            shipmentRepo = ShipmentRepo;
            this.configuration = configuration;
            this.mapper = mapper;
            this.countryRepository = countryRepository;
            this.cityRepository = cityRepository;
            this.categoryRepository = categoryRepository;
            this.productRepo = productRepo;
            this.userManager = userManager;
        }
        [ProducesResponseType(typeof(ShipmentToDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]

        public async Task<ActionResult<IEnumerable<ShipmentToDto>>> GetShipments([FromQuery] TripwShSpecParams tripwShSpec)
        {
            //var trip= await tripRepo.GetAllAsync();

            var spec = new ShipmentSpecification(tripwShSpec);
            var shipments = await shipmentRepo.GetAllWithSpecAsyn(spec);

            if (shipments is null) return NotFound(new ApiResponse(404));
            var shipmentDtos = new List<ShipmentToDto>();
            foreach (var shipment in shipments)
            {
                var shipmentDto = mapper.Map<Shipment, ShipmentToDto>(shipment);

                // استخدم UserManager للبحث عن اسم المستخدم باستخدام معرّف المستخدم
                var user = await userManager.FindByIdAsync(shipment.IdentityUserId);
                if (user != null)
                {
                    shipmentDto.UserPicture = new PictureUserResolver(configuration).Resolve(user, null, null, null);
                    shipmentDto.UserName = user.DisplayName; // افترضت هنا أن DisplayName هو الخاصية التي تحمل اسم المستخدم
                }
                else
                {
                    shipmentDto.UserName = "Unknown"; // إذا لم يتم العثور على المستخدم
                }

                shipmentDtos.Add(shipmentDto);
            }
            //var data = mapper.Map<IEnumerable<Shipment>, IEnumerable<ShipmentToDto>>(shipments);
            var countSpec = new shipmentsWithFilterForCountSpecification(tripwShSpec);
            var Count = await shipmentRepo.GetCountWithSpecAsync(countSpec);
            return Ok(new Pagination<ShipmentToDto>(tripwShSpec.PageIndex, tripwShSpec.PageSize, Count, shipmentDtos));

        }
        [ProducesResponseType(typeof(ShipmentToDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]

        public async Task<ActionResult<ShipmentToDto>> GetShipment(int id)
        {
            var spec = new ShipmentSpecification(id);
            var shipment = await shipmentRepo.GetByIdwithSpecAsyn(spec);

            if (shipment is null) return NotFound(new ApiResponse(404));
            var mappedshipment = mapper.Map<Shipment, ShipmentToDto>(shipment);
            var user = await userManager.FindByIdAsync(shipment.IdentityUserId);
            if (user != null)
             {
                mappedshipment.UserName = user.DisplayName;
                mappedshipment.UserPicture = new PictureUserResolver(configuration).Resolve(user, null, null, null);
            }
            return Ok(mappedshipment);
        }
        [Authorize]
        [HttpPost("CreateShipment")]
        public async Task<ActionResult<Shipment>> CreateShipment([FromForm] ShipmentToDto shipmentCreateDto, [FromForm] IFormFile images)
        {
            if (ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);

                var existingUser = await userManager.FindByEmailAsync(email);

                var fromCity = await cityRepository.GetCityByNameAsync(shipmentCreateDto.FromCityName);
                var fromCountry = await countryRepository.GetCountryByNameAsync(shipmentCreateDto.CountryNameFrom);
                var toCity = await cityRepository.GetCityByNameAsync(shipmentCreateDto.ToCityName);
                var toCountry = await countryRepository.GetCountryByNameAsync(shipmentCreateDto.CountryNameTo);
                var categoryName = shipmentCreateDto.Products.Select(p => p.CategoryName).FirstOrDefault();
                var category2 = await categoryRepository.GetCategoryByNameAsync(categoryName);
              
                if (fromCity != null && fromCountry != null && toCity != null && toCountry != null && category2 != null && existingUser != null && shipmentCreateDto.DateOfRecieving > DateTime.Now)
                {
                    var mappedShipment = mapper.Map<ShipmentToDto, Shipment>(shipmentCreateDto);

                    mappedShipment.FromCityID = fromCity.Id;
                    mappedShipment.FromCity = fromCity;
                    mappedShipment.FromCity.CountryId = fromCountry.Id;
                    mappedShipment.ToCityId = toCity.Id;
                    mappedShipment.ToCity = toCity;
                    mappedShipment.ToCity.CountryId = toCountry.Id;
      

                    

                            mappedShipment.IdentityUserId = existingUser?.Id;

                    if (images != null &&  shipmentCreateDto.Products != null && shipmentCreateDto.Products.Count > 0)
                    {


                        // لحفظ عناوين الصور للمنتجات
                        // لحفظ عناوين الصور للمنتجات
                        var productImageUrls = new List<string>();

                       
                            var productImageUrl = DocumentSetting.UploadImage(images, "products");
                            productImageUrls.Add(productImageUrl);
                        

                        // إضافة المنتجات مع عناوين الصور المناسبة إلى الشحنة
                        foreach (var productDto in shipmentCreateDto.Products)
                        {
                            var product = mapper.Map<Product>(productDto);
                            var productImageUrll = DocumentSetting.UploadImage(images, "products");

                            // استرجاع المنتج إذا كان موجودًا بالفعل في الشحنة
                            var existingProduct = mappedShipment.Products.FirstOrDefault(p => p.Id == productDto.ProductId);
                            if (existingProduct != null)
                            {
                                // تحديث التفاصيل الخاصة بالمنتج
                                existingProduct.PictureUrl = productImageUrll;
                                existingProduct.CategoryId = category2.Id;
                                existingProduct.Category = category2;
                            }
                            else
                            {
                                // إذا لم يكن المنتج موجودًا بالفعل، قم بإضافته إلى الشحنة
                                product.PictureUrl = productImageUrl;
                                product.CategoryId = category2.Id;
                                product.Category = category2;
                                mappedShipment.Products.Add(product);
                            }
                        }

                        // بعد إضافة أو تحديث جميع المنتجات، احفظ الشحنة
                        await shipmentRepo.AddAsync(mappedShipment);
                        await shipmentRepo.SaveChangesAsync();


                    }


                    var newShipmentDto = mapper.Map<Shipment, ShipmentToDto>(mappedShipment);
                    return Ok(new { shipment = newShipmentDto, message = "Shipment Created Successfully" });
                }
                else
                {
                    return NotFound(new { message = "City or country not found" });
                }
            }

            // Model state is not valid
            return BadRequest(ModelState);
        }

    }
}