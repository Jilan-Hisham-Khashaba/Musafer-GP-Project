using GP.Core.Entities.Orders;

namespace AdminDashBoard.Models
{
    public class AdminCountModel
    {
        public int AdminCount{ get; set; }
        public int UserCount { get; set; }
        public int shipmentCount { get; set; }

        public int TripCount { get; set; }

        public int OrderCount { get; set; }

        public int RequestCount { get; set;}

        public List<Order> TodayOrders { get; set; }
    }
   }

