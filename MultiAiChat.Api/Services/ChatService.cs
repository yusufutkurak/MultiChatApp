using Microsoft.EntityFrameworkCore;
using MultiAiChat.Api.Data;
using MultiAiChat.Api.Models;

namespace MultiAiChat.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;

        public ChatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatSession> CreateSessionAsync()
        {
            var session = new ChatSession
            {
                Id = Guid.NewGuid(),
                Title = "New Chat " + DateTime.Now.ToString("HH:mm"),
                CreatedAt = DateTime.Now
            };

            _context.ChatSession.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<List<ChatSession>> GetAllSessionsAsync()
        {
            return await _context.ChatSession
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ChatMessage>> GetMessageBySessionIdAsync(Guid sessionId)
        {
            return await _context.ChatMessage
                .Where(x => x.SessionId == sessionId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddMessageAsync(Guid sessionId, string content, string sender, string? model = null)
        {
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Content = content,
                Sender = sender,
                Model = model,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessage.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSessionAsync(Guid sessionId)
        {
            var session = await _context.ChatSession.FindAsync(sessionId);
            if (session != null)
            {
                var messages = _context.ChatMessage.Where(x => x.SessionId == sessionId);
                _context.ChatMessage.RemoveRange(messages);

                _context.ChatSession.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearAllSessionsAsync()
        {
            var allSessions = await _context.ChatSession.ToListAsync();
            var allMessages = await _context.ChatMessage.ToListAsync();

            _context.ChatMessage.RemoveRange(allMessages);
            _context.ChatSession.RemoveRange(allSessions);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            var message = await _context.ChatMessage.FindAsync(messageId);

            if (message != null)
            {
                _context.ChatMessage.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}