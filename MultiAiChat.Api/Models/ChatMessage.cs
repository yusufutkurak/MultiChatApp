using System.Text.Json.Serialization;

namespace MultiAiChat.Api.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string? Model { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ChatSession? Session { get; set; }
    }
}