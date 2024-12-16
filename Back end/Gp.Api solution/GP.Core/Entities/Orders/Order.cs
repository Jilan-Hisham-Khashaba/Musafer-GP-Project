using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities.Orders
{
    public class Order:BaseEntity
    {
        //must be parameterless constructor
        public Order()
        {
            
        }
        public Order(string email,Address? shippingAddress, int requestId)
        {
            Email = email;
            ShippingAddress = shippingAddress;
            RequestId = requestId;
        }
       
        public string Email { get; set; } 

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Address? ShippingAddress { get; set; }

        public DateTime? DateOfCreation { get; set; } = DateTime.Now;

        public string recievedUserId { get; set; }

        // public DateTime? OrderRecievied { get; set; } = Request.Trip.arrivalTime.Value;


    }
}
