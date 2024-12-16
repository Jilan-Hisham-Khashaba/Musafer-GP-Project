using AdminDashBoard.Models;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Entities.Orders;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace AdminDashBoard.Controllers
{
 
    public class HomeController : Controller
    {
        
        
        private readonly UserManager<AppUser> userManager;
        private readonly IGenericRepositroy<Order> orderRepo;
        private readonly IRequestRepository requestRepository;
        private readonly ShipmentCountSpecification shipmentCountSpecification;
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly IGenericRepositroy<Trip> tripRepo;

        public HomeController(UserManager<AppUser> userManager,IRequestRepository requestRepository,IGenericRepositroy<Order> orderRepo ,ShipmentCountSpecification shipmentCountSpecification, IGenericRepositroy<Shipment> shipmentRepo,IGenericRepositroy<Trip> tripRepo)
        {
            this.userManager = userManager;
            this.orderRepo = orderRepo;
            this.requestRepository = requestRepository;
            this.shipmentCountSpecification = shipmentCountSpecification;
            this.shipmentRepo = shipmentRepo;
            this.tripRepo = tripRepo;
        }
       

        [Authorize]
        [HttpGet]
        public async Task< IActionResult> Index()
        {
            var adminCount = (await userManager.GetUsersInRoleAsync("Admin")).Count;
            var userCount = userManager.Users.Count();
            var shipmentCountSpec = new ShipmentCountSpecification(shipmentRepo);
            var shipmentCount = await shipmentCountSpec.GetAllShipmentCountAsync();
            var tripCountSpec = new TripCountSpecification(tripRepo);
            var tripCount = await tripCountSpec.GetTripAllCountAsync();
            var requestCount = (await requestRepository.GetAllRequestsAsyncCount()).Count();
            var orderCount = (await orderRepo.GetAllAsync()).Count();
            var todayOrders = await orderRepo.GetOrdersCreatedTodayAsync();


            var dto = new AdminCountModel
            {
                AdminCount= adminCount,
                UserCount = userCount,
                shipmentCount=shipmentCount,
                TripCount= tripCount,
                RequestCount= requestCount,
                OrderCount= orderCount,
                TodayOrders= todayOrders,
            };

            return View(dto);
        }
       
     
       
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
