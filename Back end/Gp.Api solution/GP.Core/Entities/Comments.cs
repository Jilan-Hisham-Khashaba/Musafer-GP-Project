using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP.core.Entities.identity;

namespace GP.Core.Entities
{
    public class Comments : BaseEntity
    {

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal Rate { get; set; }
        public string UserId { get; set; }

        public string SenderId { get; set; }

       
    
    }
}
