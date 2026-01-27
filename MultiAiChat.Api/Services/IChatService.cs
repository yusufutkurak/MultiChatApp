using MultiAiChat.Api.Models;

namespace MultiAiChat.Api.Services
{
    public interface IChatService
    {
        Task<ChatSession> CreateSessionAsync();
        Task<List<ChatSession>> GetAllSessionsAsync();
        Task<List<ChatMessage>> GetMessageBySessionIdAsync(Guid sessionId);

        Task AddMessageAsync(Guid sessionId, string content, string sender, string? model = null);

        Task DeleteSessionAsync(Guid sessionId);
        Task ClearAllSessionsAsync();

        Task DeleteMessageAsync(Guid messageId);
    }
}