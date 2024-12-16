using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class City:BaseEntity
    {
        public string NameOfCity { get; set; }

     
        public int CountryId { get; set; }

        public Country Country { get; set; }

        //Navigation property to represent shipments from this city
        //public ICollection<Shipment> ShipmentsFrom { get; set; } = new HashSet<Shipment>();


        //// Navigation property to represent shipments to this city
        //public ICollection<Shipment> ShipmentsTo { get; set; } = new HashSet<Shipment>();


        ////Navigation property to represent shipments from this city
        //public ICollection<Trip> TripFrom { get; set; } = new HashSet<Trip>();


        //// Navigation property to represent shipments to this city
        //public ICollection<Trip> TripTo { get; set; } = new HashSet<Trip>();




    }
}
