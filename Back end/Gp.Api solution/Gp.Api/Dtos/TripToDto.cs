namespace Gp.Api.Dtos
{
    public class TripToDto
    {
       public int Id { get; set; }

        public decimal? availableKg { get; set; }

        public DateTime? arrivalTime { get; set; }

        public DateTime? dateofCreation { get; set; }=DateTime.Now;

        public int? FromCityID { get; set; }
        public string? FromCityName { get; set; }

        public int? CountryIdFrom { get; set; }

        public string? CountryNameFrom { get; set; }

        public int? ToCityId { get; set; }

        public string? ToCityName { get; set; }

        public int? CountryIdTo { get; set; }

        public string? CountryNameTo { get; set; }

        public decimal? UnitPricePerKg { get; set; }



        public decimal? SubTotal { get; set; }




        public decimal? GetTotal() => SubTotal;
        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserPhoto { get; set;}
    }
}
