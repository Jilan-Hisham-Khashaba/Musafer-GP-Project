using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class NotifactionMessage : BaseEntity
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsBroadcast { get; set; }

        public bool IsRead { get; set; }

        public string RecipientId { get; set; } 
    }
}
