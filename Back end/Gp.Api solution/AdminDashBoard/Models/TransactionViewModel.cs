namespace AdminDashBoard.Models
{
    public class TransactionViewModel
    {
        public List<ShipmentView> Shipments { get; set; }
        public List<TripViewModel>? Trips { get; set; }
    }
}
