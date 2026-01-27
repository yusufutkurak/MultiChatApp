using MultiAiChat.Api.Models;

namespace MultiAiChat.Api.Services
{
    public interface IAiService
    {
        Task<string> GetResponseAsync(List<ChatMessage> conversationHistory, string model);
    }
}