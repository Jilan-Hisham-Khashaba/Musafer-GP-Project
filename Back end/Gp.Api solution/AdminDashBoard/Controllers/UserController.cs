using AdminDashBoard.Helpers;
using AdminDashBoard.Models;
using AutoMapper;
using Emgu.CV.Ocl;
using Gp.Api.Dtos;
using Gp.Api.Hellpers;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Entities.Orders;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using GP.Repository.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace AdminPanal.Controllers
{

    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IFaceComparisonResultRepository faceComparisonResultRepository;
        private readonly ShipmentCountSpecification shipmentCountSpecification;
        private readonly StoreContext context;
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly IGenericRepositroy<Trip> tripRepo;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<AppUser> userManager,IMapper mapper,IConfiguration configuration,IFaceComparisonResultRepository faceComparisonResultRepository ,ShipmentCountSpecification shipmentCountSpecification, StoreContext context, IGenericRepositroy<Shipment> shipmentRepo, IGenericRepositroy<Trip> tripRepo, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.configuration = configuration;
            this.faceComparisonResultRepository = faceComparisonResultRepository;
            this.shipmentCountSpecification = shipmentCountSpecification;
            this.context = context;
            this.shipmentRepo = shipmentRepo;
            this.tripRepo = tripRepo;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            var userViews = new List<UserView>();


            foreach (var user in users)
            {
                var shipmentCountSpec = new ShipmentCountSpecification(shipmentRepo);
                var shipmentCount = await shipmentCountSpec.GetShipmentCountAsync(user.Id);
                var TripCountSpec = new TripCountSpecification(tripRepo);
                var TripCount = await TripCountSpec.GetTripCountAsync(user.Id);

                var isAdmin = await userManager.IsInRoleAsync(user, "admin");
                if (!isAdmin)
                {
                    var userView = new UserView
                    {
                        IsEnableFactor=user.TwoFactorEnabled,
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        LastLogin = user?.LastLogin,
                        ShipmentCount = shipmentCount,
                        TripCount = TripCount,
                    };

                    userViews.Add(userView);
                }
            }

            return View(userViews);
        }

        public async Task<IActionResult> Transaction(string id)
        {
            // Retrieve the user by id
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            var shipmentSpec = new ShipmentSpecification(id);
            var shipments = await shipmentRepo.GetAllWithSpecAsyn(shipmentSpec);


            var tripSpec = new TripSpecifications(id);
            var trips = await tripRepo.GetAllWithSpecAsyn(tripSpec);

            var transactionViewModels = new List<TransactionViewModel>();


            if (shipments.Any() && trips.Any())
            {

                foreach (var shipment in shipments)
                {
                    foreach (var trip in trips)
                    {
                        var viewModel = new TransactionViewModel
                        {
                            Shipments = new List<ShipmentView> { new ShipmentView
                    {
                        fromCity = shipment.FromCity.NameOfCity,
                        ToCity = shipment.ToCity.NameOfCity,
                        userName = user.UserName,
                        date = shipment.DateOfRecieving,
                        weight = shipment.Weight,
                        Products = shipment.Products.Select(product => new ProductsView
                        {
                            ProductName = product.ProductName,
                            ProductPrice = product.ProductPrice
                        }).ToList()
                    }},
                            Trips = new List<TripViewModel> { new TripViewModel
                    {
                        fromCity = trip.FromCity.NameOfCity,
                        ToCity = trip.ToCity.NameOfCity,
                        weightTrip = trip.availableKg,
                        Reward = trip.UnitPricePerKg,
                        userName = user.UserName
                    }}
                        };

                        transactionViewModels.Add(viewModel);
                    }
                }
            }

            else if (shipments.Any() && !trips.Any())
            {

                foreach (var shipment in shipments)
                {
                    var viewModel = new TransactionViewModel
                    {
                        Shipments = new List<ShipmentView> { new ShipmentView
                {
                    fromCity = shipment.FromCity.NameOfCity,
                    ToCity = shipment.ToCity.NameOfCity,
                    userName = user.UserName,
                    date = shipment.DateOfRecieving,
                    weight = shipment.Weight,
                    Products = shipment.Products.Select(product => new ProductsView
                    {
                        ProductName = product.ProductName,
                        ProductPrice = product.ProductPrice
                    }).ToList()
                }},
                        Trips = new List<TripViewModel>() // No trips for this user
                    };

                    transactionViewModels.Add(viewModel);
                }
            }
            // If user has only trips
            else if (!shipments.Any() && trips.Any())
            {
                // Populate the list with TransactionViewModel instances for each trip
                foreach (var trip in trips)
                {
                    var viewModel = new TransactionViewModel
                    {
                        Shipments = new List<ShipmentView>(), // No shipments for this user
                        Trips = new List<TripViewModel> { new TripViewModel
                {
                    fromCity = trip.FromCity.NameOfCity,
                    ToCity = trip.ToCity.NameOfCity,
                    weightTrip = trip.availableKg,
                    Reward = trip.UnitPricePerKg,
                    userName = user.UserName
                }}
                    };

                    transactionViewModels.Add(viewModel);
                }
            }

            // Pass the list to the view
            return View(transactionViewModels);
        }
        public async Task<ActionResult> Verify(string id)
        {
            // Retrieve the user by id
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var shipmentCountSpec = new ShipmentCountSpecification(shipmentRepo);
            var shipmentCount = await shipmentCountSpec.GetShipmentCountAsync(user.Id);
            var tripCountSpec = new TripCountSpecification(tripRepo);
            var tripCount = await tripCountSpec.GetTripCountAsync(user.Id);

            // Fetch face verification details
            var userExistsInVerificationFaces = await faceComparisonResultRepository.CheckUserExistsInVerificationFaces(user.Id);
            var faceComparisonResult = userExistsInVerificationFaces ? await faceComparisonResultRepository.GetFaceComparisonResultByUserId(user.Id) : null;

            var userView = new UserView
            {
                Id = user.Id,
                IsEnableFactor = user.TwoFactorEnabled,
                UserName = user.UserName,
                Email = user.Email,
                LastLogin = user?.LastLogin,
                ShipmentCount = shipmentCount,
                PhoneNumber = user.PhoneNumber,
                City = user.Address?.City ?? "No",
                TripCount = tripCount,
                image = new UserImageResolver(configuration).Resolve(user, null, null, null),
                FacessAccuracy = faceComparisonResult?.accuracy ?? 0, // Assuming Accuracy is a property of FaceComparison
                MatchStatus = faceComparisonResult?.MatchStatus ?? "No Data" // Assuming MatchStatus is a property of FaceComparison
            };
            var vrefactionFacesDto = new VrefactionFacesDto();

            var mappedVerfication = mapper.Map<VrefactionFacesDto, verficationFaccess>(vrefactionFacesDto);
            // Pass the verification image URL to the view model if it exists
            if (faceComparisonResult != null)
            {
                var imageResolver = new ImageVerficationResolver(configuration);
                userView.VerfiyImage = imageResolver.Resolve(mappedVerfication, null, null, null);
            }

            return View(userView);
        }

        [HttpPost]
    
        public async Task<IActionResult> UpdateTwoFactorStatus([FromBody] UpdateTwoFactorStatusModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Id))
                return BadRequest(new { success = false, message = "Invalid user ID" });

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            user.TwoFactorEnabled = model.IsEnableFactor;
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { success = true });
            else
                return BadRequest(new { success = false, message = "Failed to update user" });
        }

        public class UpdateTwoFactorStatusModel
        {
            public string Id { get; set; }
            public bool IsEnableFactor { get; set; }
        }




    }

}


