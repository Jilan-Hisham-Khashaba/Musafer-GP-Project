using System.ComponentModel.DataAnnotations;

namespace Gp.Api.Dtos
{
    public class RequestDto
    {
        private DateTime? dateOFCreation = DateTime.Now;

        public int? Id { get; set; }

        public ShipmentToDto ShipmentToDto { get; set; }

        public TripToDto TripToDto { get; set; }

        public string? RequestUserId { get; set; }

        public string? SenderUserId { get; set; }


        public bool? ISConvertedToOrder { get; set; }
        public DateTime? DateOFCreation { get ; set; }=DateTime.Now;
    }
}