namespace MultiAiChat.Api.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "Yeni Sohbet";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}