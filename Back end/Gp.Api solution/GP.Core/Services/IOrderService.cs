using GP.Core.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string Email, int requestId,Address shippingAddress);

        Task<List<Order>> GetOrdersForUserAsync(string Email);

        Task<List<Order>> GetAllOrdersAsync();

        Task<Order> GetOrderByIdAsync(int orderId);

        Task<Order> GetOrderByIdForUserAsync(int orderId,string email);

        Task<string> SerializeOrderToJsonAsync(Order order);

         Task<bool> OrderExistsForRequest(int requestId);



}
}
