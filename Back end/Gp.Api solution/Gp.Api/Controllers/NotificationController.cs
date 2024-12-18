using Emgu.CV.Ocl;
using Gp.Api.Dtos;
using GP.core.Entities.identity;
using GP.Repository.Data;
using GP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class NotificationController : ApiBaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IHubContext<ChatHub> chatHub;
        private readonly StoreContext context;

        public NotificationController(UserManager<AppUser> userManager , IHubContext<ChatHub> chatHub, StoreContext context)
        {
            this.userManager = userManager;
            this.chatHub = chatHub;
            this.context = context;
        }
        [Authorize]
        [HttpGet("notifications")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // جلب الإشعارات
            var notifications = await context.NotifactionMessage
                .Where(n => n.RecipientId == existingUser.Id && !n.IsBroadcast && n.IsRead)
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();

        
            if (notifications == null || notifications.Count == 0)
            {
                return NotFound(new { message = "No notifications found" });
            }

       
            var notificationDtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Content,
                IsBroadcast = n.IsBroadcast,
                IsRead = n.IsRead,
                CreatedAt = n.Timestamp
            }).ToList();

            return Ok(notificationDtos);
        }

        [Authorize]
        [HttpGet("latest-notification")]
        public async Task<ActionResult<NotificationDto>> GetLatestNotification()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);

            var latestNotification = await context.NotifactionMessage
                .Where(n => n.RecipientId == existingUser.Id && !n.IsBroadcast && !n.IsRead)
                .OrderByDescending(n => n.Timestamp)
                .FirstOrDefaultAsync();

            if (latestNotification == null)
            {
                return NotFound(); 
            }

          
            latestNotification.IsRead = true;
            await context.SaveChangesAsync();

            var latestNotificationDto = new NotificationDto
            {
                Id = latestNotification.Id,
                Message = latestNotification.Content,
                IsRead = latestNotification.IsRead,
                CreatedAt = latestNotification.Timestamp
            };

            return Ok(latestNotificationDto);
        }

        [Authorize]
        [HttpGet("read-notifications")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetReadNotifications()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // جلب الإشعارات المقروءة
            var notifications = await context.NotifactionMessage
                .Where(n => n.RecipientId == existingUser.Id && !n.IsBroadcast && n.IsRead)
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();

            if (notifications == null || notifications.Count == 0)
            {
                return NotFound(new { message = "No read notifications found" });
            }

            var notificationDtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Content,
                IsBroadcast = n.IsBroadcast,
                IsRead = n.IsRead,
                CreatedAt = n.Timestamp
            }).ToList();

            return Ok(notificationDtos);
        }


    }
}
