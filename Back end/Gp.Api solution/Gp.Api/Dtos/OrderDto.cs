using GP.Core.Entities.Orders;
using System.ComponentModel.DataAnnotations;

namespace Gp.Api.Dtos
{
    public class OrderDto
    {
        [Required]
        public int RequestId { get; set; }

        public int? id { get; set; }

        public AddressDto? ShippingAddress { get; set; }

        public string? RecievedUserId { get; set; }

        public string? email { get; set; }

        public string? senderid { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;
        public RequestDto? Request { get; set; }
        public int Status { get; set; }
      



    }
}
