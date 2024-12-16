using Gp.Api.Dtos;

namespace AdminDashBoard.Models
{
    public class ShipmentView
    {
        public string fromCity { get; set; }

        public decimal weight { get; set; }

        public string ToCity { get; set; }

        public int? count { get; set; }

        public string categoryName { get; set; }

    
        public int RequestId { get; set; }
        public decimal Reward { get; set; }

        public DateTime date { get; set; }

        public List<ProductsView> Products { get; set; }

        public string userName { get; set; }

        public string userId { get; set; }
    }
}
