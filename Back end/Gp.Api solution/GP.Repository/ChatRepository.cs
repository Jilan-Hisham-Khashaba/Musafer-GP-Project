using Emgu.CV.Ocl;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly StoreContext _context;

        public ChatRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesForUser(string userId)
        {
            return await _context.ChatMessages
            .Where(m => m.Receiver == userId || m.Sender == userId)
            .ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetMessageReplies(int messageId)
        {
            return await _context.ChatMessages.Where(m => m.ReplyToMessageId == messageId).ToListAsync();
        }

        public async Task UpdateMessageStatus(int messageId, bool isRead)
        {
            var message = await _context.ChatMessages.FindAsync(messageId);
            if (message != null)
            {
                message.Seen = isRead;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ChatMessage> GetMessageById(int messageId)
        {
            return await _context.ChatMessages.FindAsync(messageId);
        }
        public async Task<IEnumerable<ChatMessage>> GetSentMessages(string userId)
        {
            return await _context.ChatMessages
                                .Where(m => m.Sender == userId)
                                .ToListAsync();
        }
        public async Task<IEnumerable<ChatMessage>> GetMessagesBetweenUsers(string userId1, string userId2)
        {
            return await _context.ChatMessages
                .Where(m => (m.Sender == userId1 && m.Receiver == userId2) || (m.Sender == userId2 && m.Receiver == userId1))
                .ToListAsync();
        }

        public async Task AddAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

       
    }
}

