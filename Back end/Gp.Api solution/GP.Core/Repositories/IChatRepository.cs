using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Repositories
{
    public interface IChatRepository
    {
        Task AddAsync(ChatMessage message);
        Task<ChatMessage> GetMessageById(int messageId);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<ChatMessage>> GetMessagesForUser(string userId);
        Task<IEnumerable<ChatMessage>> GetMessageReplies(int messageId);
        Task<IEnumerable<ChatMessage>> GetSentMessages(string userId);
        Task UpdateMessageStatus(int messageId, bool isRead);
        Task<IEnumerable<ChatMessage>> GetMessagesBetweenUsers(string userId1, string userId2); // New method
    }
}
