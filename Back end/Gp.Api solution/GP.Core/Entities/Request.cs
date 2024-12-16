using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class Request 
    {

        public Request()
        {
            // يمكنك ترك هذا الكونستركتور فارغًا
        }
        public Request(int  id)
        {
            this.RequestId = id;
        }

        public int  RequestId { get; set; }

        // public int ShipmentId { get; set; }
        //public List<Shipment> Shipment { get; set; }

        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public int TripId { get; set; }
       public Trip Trip { get; set; }

       public string UserId { get; set; }

        public string? SenderId { get; set; }

        public bool IsConvertedToOrder { get; set; } = false;

        public DateTime? DateOfCreation { get; set; }= DateTime.Now;
    }
}
