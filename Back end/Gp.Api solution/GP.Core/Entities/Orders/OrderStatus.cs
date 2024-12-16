using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities.Orders
{
    public enum OrderStatus
    {//int
        [EnumMember(Value ="Pending")]
        Pending,
        [EnumMember(Value = "Recived")]
        Received,
        [EnumMember(Value = "Failed")]
        Failed,

    }
}
