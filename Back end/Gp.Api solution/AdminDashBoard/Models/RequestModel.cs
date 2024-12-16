namespace AdminDashBoard.Models
{
    public class RequestModel
    {
        public int Id { get; set; }

        public DateTime? dateOfCreation { get; set; }

        public string RequestId { get; set; }

        public string type { get; set; }

        public int? senderId { get; set; }

        public List<ShipmentView> ShipmentViews { get; set; }

        public List<TripViewModel> TripViews { get; set; }

    }
}
