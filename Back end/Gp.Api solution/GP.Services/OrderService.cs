using GP.Core.Entities;
using GP.Core.Entities.Orders;
using GP.Core.Repositories;
using GP.Core.Services;
using GP.Core.Specificatios.Order_Spec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GP.core.Entities.identity;
using Emgu.CV.Ocl;
using Microsoft.EntityFrameworkCore.Metadata;
using GP.Repository.Data;

namespace GP.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRequestRepository requestRepository;
        private readonly IChatHub chatHub;
        private readonly StoreContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
        private readonly IGenericRepositroy<Trip> tripRepo;
        private readonly IGenericRepositroy<Order> orderRepository;

        public OrderService(IRequestRepository requestRepository, IChatHub _chatHub,StoreContext context ,UserManager<AppUser> userManager, IGenericRepositroy<Shipment> shipmentRepo, IGenericRepositroy<Trip> TripRepo, IGenericRepositroy<Order> orderRepository)
        {
            this.requestRepository = requestRepository;
            chatHub = _chatHub;
            this.context = context;
            this.userManager = userManager;
            this.shipmentRepo = shipmentRepo;
            tripRepo = TripRepo;
            this.orderRepository = orderRepository;
        }

        public async Task<bool> OrderExistsForRequest(int requestId)
        {
            var existingOrder = await orderRepository.FirstOrDefaultAsync(o => o.RequestId == requestId);
            return existingOrder != null;
        }




        public async Task<Order> CreateOrderAsync(string Email, int requestId, Core.Entities.Orders.Address shippingAddress)
        {
            if (await OrderExistsForRequest(requestId))
            {
                throw new Exception("An order already exists for the specified request.");
            }
            var request = await requestRepository.GetRequestAsync(requestId);
            if (request == null)
            {
                throw new Exception("Request not found.");
            }
            if (request.Trip != null && request.Shipment != null)
            {
                var order = new Order(Email, shippingAddress, requestId);
                var receivedUserId = request.Trip.UserId != Email ? request.Trip.UserId : request.Shipment.IdentityUserId;
                order.recievedUserId = receivedUserId;
                await orderRepository.AddAsync(order);
                if (request.Shipment != null)
                {
                    context.Entry(request.Shipment).State = EntityState.Detached;
                    if (request.Shipment.FromCity != null)
                        context.Entry(request.Shipment.FromCity).State = EntityState.Detached;
                    if (request.Shipment.ToCity != null)
                        context.Entry(request.Shipment.ToCity).State = EntityState.Detached;
                }

                if (request.Trip != null)
                {
                    context.Entry(request.Trip).State = EntityState.Detached;
                    if (request.Trip.FromCity != null)
                        context.Entry(request.Trip.FromCity).State = EntityState.Detached;
                    if (request.Trip.ToCity != null)
                        context.Entry(request.Trip.ToCity).State = EntityState.Detached;
                }
                request.IsConvertedToOrder = true;
                await requestRepository.UpdateRequestAsync2(request);
                var tripResult = await tripRepo.GetByIdAsyncTRIP(request.TripId);
                var trip = tripResult;

                if (trip.availableKg >= request.Shipment.Weight)
                {
                    trip.availableKg -= request.Shipment.Weight;
                    await orderRepository.SaveChangesAsync();
                    tripRepo.Update(trip);
                    await tripRepo.SaveChangesAsync();
                    var user = await userManager.FindByIdAsync(Email);
                    var userEmail = user.UserName;
                    var notificationMessage = $"You have accepted a request from {userEmail}. Check this order.";
                    await chatHub.SendNotification(receivedUserId, notificationMessage);
                    return order;
                }
                else
                {
                    throw new Exception("Selected trip has insufficient capacity for this shipment.");
                }
            }
            else
            {
                throw new Exception("Trip or shipment is missing for the specified request.");
            }
        }




        public async Task<Order> GetOrderByIdForUserAsync(int orderId,string email)
        {
            var spec = new OrderSpecification(orderId, email);
            var order = await orderRepository.GetByIdwithSpecAsyn(spec);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // Serialize the list of orders to JSON
            var jsonString = JsonSerializer.Serialize(order, options);

            // Deserialize the JSON back to a list of orders
            var deserializedOrders = JsonSerializer.Deserialize<Order>(jsonString, options);

            return deserializedOrders;


        }
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var spec = new OrderSpecification(orderId);
            var order = await orderRepository.GetByIdwithSpecAsyn(spec);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // Serialize the list of orders to JSON
            var jsonString = JsonSerializer.Serialize(order, options);

            // Deserialize the JSON back to a list of orders
            var deserializedOrders = JsonSerializer.Deserialize<Order>(jsonString, options);

            return deserializedOrders;


        }

        public async Task<List<Order>> GetOrdersForUserAsync(string Email)
        {
            var spec = new OrderSpecification(Email);
            spec.includes.Add(o => o.Request.Shipment.Products);
            var orders = await orderRepository.GetAllWithSpecAsyn(spec);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // Serialize the list of orders to JSON
            var jsonString = JsonSerializer.Serialize(orders, options);

            // Deserialize the JSON back to a list of orders
            var deserializedOrders = JsonSerializer.Deserialize<List<Order>>(jsonString, options);

            return deserializedOrders;
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            // Assuming there's a method in your repository to retrieve all orders
            var spec = new OrderSpecification();
            spec.includes.Add(o => o.Request.Shipment.Products);
            var orders = await orderRepository.GetAllWithSpecAsyn(spec) ;

       
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // Serialize the list of orders to JSON
            var jsonString = JsonSerializer.Serialize(orders, options);

            // Deserialize the JSON back to a list of orders
            var deserializedOrders = JsonSerializer.Deserialize<List<Order>>(jsonString, options);

            return deserializedOrders;
        }



        public async Task<string> SerializeOrderToJsonAsync(Order order)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var jsonString = JsonSerializer.Serialize(order, options);

            return jsonString;
        }

    }
}
