using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class Product:BaseEntity
    {
        public string ProductName { get; set; }

        public string? ProductUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductWeight { get; set; }
        
        public string? PictureUrl { get; set; }

        //[NotMapped]
        //public IFormFile? Image { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }


        public ICollection<Shipment> shipments { get; set; } = new HashSet<Shipment>();




    }
}
