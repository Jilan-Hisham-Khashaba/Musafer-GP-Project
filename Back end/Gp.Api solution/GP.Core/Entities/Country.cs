using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class Country:BaseEntity
    {
        public string NameCountry { get; set; }

        public string? Contient { get; set; }

       
        public ICollection<City> cities { get; set; } = new HashSet<City>();
    }
}
