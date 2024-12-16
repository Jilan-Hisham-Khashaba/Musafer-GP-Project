using AdminDashBoard.Models;
using Gp.Api.Dtos;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace AdminDashBoard.Controllers
{
    public class RequestController : Controller
    {
        private readonly IRequestRepository requestRepository;
        private readonly UserManager<AppUser> userManager;
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly IGenericRepositroy<Trip> tripRepo;

        public RequestController(IRequestRepository requestRepository,UserManager<AppUser> userManager,IGenericRepositroy<Shipment> shipmentRepo,IGenericRepositroy<Trip>TripRepo)
        {
            this.requestRepository = requestRepository;
            this.userManager = userManager;
            this.shipmentRepo = shipmentRepo;
            tripRepo = TripRepo;
        }
        [Authorize]
    
        public async Task<IActionResult> Index(string searchType)
        {
            // Get all requests
            var requests = await requestRepository.GetAllRequestsAsync();

            // قائمة لتخزين البيانات التي سيتم عرضها في العرض
            var requestDataList = new List<RequestModel>();

            // تحويل البيانات من كل طلب إلى الـ RequestModel
            foreach (var request in requests)
            {
                var requestData = new RequestModel
                {
                    Id = request.RequestId,
                    dateOfCreation = request.DateOfCreation,
                    senderId = request.UserId != request.Shipment?.IdentityUserId ? request.ShipmentId :
                               request.UserId != request.Trip?.UserId ? request.TripId :
                               0,
                    // افحص نوع العنصر باستخدام الـ requestId وقم بتعيين القيمة المناسبة للـ type
                    type = request.UserId == request.Shipment?.IdentityUserId ? "Shipment" :
                           request.UserId == request.Trip?.UserId ? "Trip" :
                           "Unknown"
                };

                // أضف البيانات إلى القائمة
                requestDataList.Add(requestData);
            }

            // إذا كان searchType غير فارغ، قم بتصفية القائمة بناءً على searchType
            if (!string.IsNullOrEmpty(searchType))
            {
                requestDataList = requestDataList.Where(r => r.type.Contains(searchType, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // قم بتمرير القائمة إلى العرض
            return View(requestDataList);
        }

        public async Task<IActionResult> details(int id)
        {
            var request = await requestRepository.GetRequestAsyncForAdmin(id);

            if (request != null)
            {
                ShipmentView shipmentData = null;
                TripViewModel tripData = null;
             
                var user = await userManager.FindByIdAsync(request.UserId);
                if (request.UserId == request.Shipment?.IdentityUserId || request.UserId == request.Trip?.UserId)
                {
                   
                    shipmentData = new ShipmentView
                    {
                        RequestId = request.RequestId,

                        fromCity = request.Shipment.FromCity.NameOfCity,
                        ToCity = request.Shipment.ToCity.NameOfCity,
                        weight = request.Shipment.Weight,
                        count=request.Shipment.Products.Count,
                        Reward=request.Trip.UnitPricePerKg,
                         userName=user.UserName,
                         Products =request.Shipment.Products.Select(product => new ProductsView
                         {
                             
                             ProductName = product.ProductName,
                             ProductPrice = product.ProductPrice,
                             
                         }).ToList()
                         
                    };
                }
                //else if (request.UserId == request.Trip?.UserId)
                //{
                //    tripData = new TripViewModel
                //    {
                //        fromCity = request.Trip.FromCity.NameOfCity,
                //        ToCity = request.Trip.ToCity.NameOfCity,
                //        weightTrip = request.Trip.availableKg,
                //        Reward = request.Trip.UnitPricePerKg
                //    };
                //}

                var viewModel = new DeatielsRequest
                {
                    Shipments = shipmentData,
                    Trips = tripData
                };

                return View(viewModel);
            }

            return View("NotFound");
        }
        public ActionResult Delete(int id)
        {
            var request = requestRepository.GetRequestAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           
            var request = await requestRepository.GetRequestAsync(id);
            if (request == null)
            {
                return NotFound();
            }

          

            try
            {
                // حذف الـ City
                await requestRepository.DeleteRequestAsync(id);

       

                TempData["SuccessMessage"] = "Request deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الخطأ وعرض رسالة خطأ
                return RedirectToAction(nameof(Index));
            }
        }




    }
}
