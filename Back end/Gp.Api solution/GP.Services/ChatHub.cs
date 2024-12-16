using Microsoft.AspNetCore.SignalR;
using GP.Core.Entities;
using Microsoft.Extensions.Logging;
using GP.Repository.Data;
using System.Threading.Tasks;
using GP.Core.Repositories;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private readonly StoreContext _context;
    private readonly IChatRepository chatRepository;

    public ChatHub(ILogger<ChatHub> logger, StoreContext context, IChatRepository chatRepository)
    {
        _logger = logger;
        _context = context;
        this.chatRepository = chatRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.Identity?.Name;

        if (!string.IsNullOrEmpty(userName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            _logger.LogInformation($"User {userName} added to group {userName}.");
        }
        else
        {
            _logger.LogWarning("User is not authenticated. Unable to add to group.");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userName = Context.User?.Identity?.Name;

        if (!string.IsNullOrEmpty(userName))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userName);
            _logger.LogInformation($"User {userName} removed from group {userName}.");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string sender, string receiver, string messageContent)
    {
        var message = new ChatMessage
        {
            Content = messageContent,
            Sender = sender,
            Receiver = receiver,
            Seen = false,
            MessageDate = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        await Clients.User(sender).SendAsync("ReceiveMessage", message);
        await Clients.User(receiver).SendAsync("ReceiveMessage", message);
    }
    public async Task MarkMessageAsRead(int messageId)
    {
        await chatRepository.UpdateMessageStatus(messageId, true);
        var message = await chatRepository.GetMessageById(messageId);
        await Clients.User(message.Sender).SendAsync("MessageRead", messageId);
    }

    public async Task SendNotification(string userId, string message)
        {
            await Clients.Group(userId).SendAsync("ReceiveNotification", message);

            var notification = new NotifactionMessage
            {
                RecipientId = userId,
                Content = message,
                IsBroadcast = false,
                IsRead = false,
                Timestamp = DateTime.UtcNow
            };

        _context.NotifactionMessage.Add(notification);
            await _context.SaveChangesAsync();
        }
    }


