using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string Content { get; set; }
        public string Sender{ get; set; }
        public string? Receiver { get; set; }
        public bool Seen { get; set; }
        public DateTime MessageDate { get; set; }
        public int? ReplyToMessageId { get; set; }
    }

}
