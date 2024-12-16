using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Services
{
    public interface IChatHub
    {
        Task SendNotification(string userId, string message);
    }
}
