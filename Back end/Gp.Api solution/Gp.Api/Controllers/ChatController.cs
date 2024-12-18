using Gp.Api.Dtos;
using GP.core.Entities.identity;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Core.Services;
using GP.Repository;
using GP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gp.Api.Controllers
{
    public class ChatController : ApiBaseController
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IChatRepository chatRepository;
        private readonly UserManager<AppUser> userManager;

        public ChatController(IHubContext<ChatHub> chatHub, IChatRepository chatRepository, UserManager<AppUser> userManager)
        {
            _chatHub = chatHub;
            this.chatRepository = chatRepository;
            this.userManager = userManager;
        }
        [Authorize]
        [HttpPost("ChatMessage")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto messageDto)
        {
            if (messageDto == null || string.IsNullOrEmpty(messageDto.Content) || string.IsNullOrEmpty(messageDto.Reciever))
            {
                return BadRequest("Message content and receiver cannot be empty");
            }

            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var sender = await userManager.FindByEmailAsync(email);
                if (sender == null)
                {
                    return BadRequest("Invalid sender user");
                }

                var receiver = await userManager.FindByIdAsync(messageDto.Reciever);
                if (receiver == null)
                {
                    return BadRequest("Invalid receiver user");
                }
                var message = new ChatMessage
                {
                    Content = messageDto.Content,
                    Sender = sender.Id,
                    Receiver = receiver.Id,
                    Seen = false,
                    MessageDate = DateTime.UtcNow
                };

                await chatRepository.AddAsync(message);
                await chatRepository.SaveChangesAsync();

                // إرسال الرسالة إلى المرسل والمستقبل
                await _chatHub.Clients.User(sender.Id).SendAsync("ReceiveMessage", message);
                await _chatHub.Clients.User(receiver.Id).SendAsync("ReceiveMessage", message);

                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request");
            }
        }
        [Authorize]
        [HttpGet("Inbox")]
        public async Task<IActionResult> GetInbox()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Unauthorized("User not found");
                }

                // Retrieve messages sent to and from the user
                var messages = await chatRepository.GetMessagesForUser(user.Id);

                // Optionally filter out messages not relevant to the user
                var filteredMessages = messages.Where(m => m.Sender == user.Id || m.Receiver == user.Id);

                return Ok(filteredMessages);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, "An error occurred while retrieving inbox messages");
            }
        }


        [Authorize]
        [HttpGet("MessageReplies/{messageId}")]
        public async Task<IActionResult> GetMessageReplies(int messageId)
        {
            try
            {
                // Retrieve replies for the specified message
                var replies = await chatRepository.GetMessageReplies(messageId);

                return Ok(replies);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, "An error occurred while retrieving message replies");
            }
        }
        [Authorize]
        [HttpGet("SentMessages")]
        public async Task<IActionResult> GetSentMessages()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var user = await userManager.FindByEmailAsync(email);

                // Retrieve messages sent by the user
                var messages = await chatRepository.GetSentMessages(user.Id);

                return Ok(messages);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, "An error occurred while retrieving sent messages");
            }
        }
        [Authorize]
        [HttpPost("UpdateMessageStatus")]
        public async Task<IActionResult> UpdateMessageStatus([FromBody] UpdateMessageStatusDto updateDto)
        {
            try
            {
                var messageId = updateDto.MessageId;
                var isRead = updateDto.IsRead;

               
                await chatRepository.UpdateMessageStatus(messageId, isRead);

                return Ok("Message status updated successfully");
            }
            catch (Exception ex)
            {
                // تسجيل أو معالجة الاستثناء
                return StatusCode(500, "An error occurred while updating message status");
            }
        }


        [Authorize]
        [HttpGet("ChatBetweenUsers")]
        public async Task<IActionResult> GetChatBetweenUsers(string userId1, string userId2)
        {
            try
            {
                // Ensure both user IDs are provided
                if (string.IsNullOrEmpty(userId1) || string.IsNullOrEmpty(userId2))
                {
                    return BadRequest("Both user IDs must be provided");
                }

                // Retrieve messages between the two users
                var messages = await chatRepository.GetMessagesBetweenUsers(userId1, userId2);

                return Ok(messages);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, "An error occurred while retrieving chat messages");
            }
        }

    }
}