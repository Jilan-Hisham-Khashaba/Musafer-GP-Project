namespace AdminDashBoard.Models
{
    public class UserView
    {
           
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? AddressCountry { get; set; }
        public string? City { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? NationalId { get; set; }
        public bool? IsEnableFactor { get; set; }
        public double? FacessAccuracy { get; set; }
        public string? MatchStatus { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? ShipmentCount { get; set; }
        public int? TripCount { get; set; }
        public string? image { get; set; }
        public string? VerfiyImage { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}

