using Emgu.CV.Ocl;
using GP.Core.Entities;
using GP.Core.Services;
using GP.Repository.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Services
{
    public class ChatHubAdapter:IChatHub
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly StoreContext context;

        public ChatHubAdapter(IHubContext<ChatHub> hubContext,StoreContext context)
        {
            _hubContext = hubContext;
            this.context = context;
        }

        public async Task SendNotification(string userId, string message)
        {
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", message);
            var notification = new NotifactionMessage
            {
                RecipientId = userId,
                Content = message,
                IsBroadcast = false,
                IsRead = false,
                Timestamp = DateTime.UtcNow
            };
            context.NotifactionMessage.Add(notification);
            await context.SaveChangesAsync();
        }
    }
}
