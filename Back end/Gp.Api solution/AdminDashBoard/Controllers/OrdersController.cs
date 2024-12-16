using AdminDashBoard.Models;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Entities.Orders;
using GP.Core.Repositories;
using GP.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Twilio.Http;

namespace AdminDashBoard.Controllers
{
   
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;
        private readonly UserManager<AppUser> userManager;
        private readonly IGenericRepositroy<Order> orderRepo;
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly IGenericRepositroy<Trip> tripRepo;
        private readonly IRequestRepository requestRepository;

        public OrdersController(IOrderService orderService, UserManager<AppUser> userManager,IGenericRepositroy<Order> OrderRepo, IGenericRepositroy<Shipment> shipmentRepo,IGenericRepositroy<Trip> tripRepo,IRequestRepository requestRepository)
        {
            this.orderService = orderService;
            this.userManager = userManager;
            orderRepo = OrderRepo;
            this.shipmentRepo = shipmentRepo;
            this.tripRepo = tripRepo;
            this.requestRepository = requestRepository;
        }
        [Authorize(Roles = "Admin")]
    
        public async Task<IActionResult> Index(string searchvalue)
        {
            // Get all orders
            var orders = await orderService.GetAllOrdersAsync();

            // قائمة لتخزين البيانات التي سيتم عرضها في العرض
            var ordersDataList = new List<OrderModels>();
            foreach (var order in orders)
            {
                var orderData = new OrderModels
                {
                    Id = order.Id,
                    dateOfCreation = order.DateOfCreation,
                    product = order.Request.Shipment.Products.Select(s => s.ProductName).FirstOrDefault(),
                    To = order.Request.Trip.ToCity.Country.NameCountry,
                    Status = order.Status.ToString(),
                    Amount = order.Request.Shipment.Products.Select(s => s.ProductPrice).FirstOrDefault(),
                };

                ordersDataList.Add(orderData);
            }
            if (!string.IsNullOrEmpty(searchvalue))
            {
                ordersDataList = ordersDataList.Where(r => r.To.Contains(searchvalue, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // قم بتمرير القائمة إلى العرض
            return View(ordersDataList);
        }



        public async Task<IActionResult> details(int id)
        {
            var order = await orderService.GetOrderByIdAsync(id);

            if (order != null)
            {
                // Retrieve the user by id
                var shipuser = await userManager.FindByIdAsync(order.Request.Trip.UserId);
                var tripuser = await userManager.FindByIdAsync(order.Request.Shipment.IdentityUserId);
                var orderData = new OrderModels
                {
                    shipName = shipuser.UserName,
                    TripName = tripuser.UserName,
                   Id= order.Id,
                    product = order.Request.Shipment.Products.Select(s => s.ProductName).FirstOrDefault(),
                    To = order.Request.Trip.ToCity.Country.NameCountry,
                    weight = order.Request.Shipment.Weight,
                    date=order.Request.Trip.arrivalTime,
                    reward=order.Request.Trip.UnitPricePerKg,
                    Amount=order.Request.Shipment.Products.Select(s=>s.ProductPrice).FirstOrDefault(),
                    Status = order.Status.ToString(),
                    tocity=order.Request.Trip.ToCity.NameOfCity,
                   

                };



                return View(orderData);
            }

            return View("NotFound");
        }
        public ActionResult Delete(int id)
        {
            var request = orderRepo.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var request = await orderRepo.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }



            try
            {
               
                await orderRepo.DeleteAsync(id);



                TempData["SuccessMessage"] = "Order deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
