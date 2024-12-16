namespace AdminDashBoard.Models
{
    public class DeatielsRequest
    {
        public int Id { get; set; } 
        public ShipmentView Shipments { get; set; }
        public TripViewModel? Trips { get; set; }
    }
}
