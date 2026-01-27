using Microsoft.AspNetCore.Mvc;
using MultiAiChat.Api.Services;
using MultiAiChat.Api.Models;

namespace MultiAiChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        private readonly GeminiService _geminiService;
        private readonly OpenAiService _openAiService;

        public ChatController(
            IChatService chatService,
            GeminiService geminiService,
            OpenAiService openAiService)
        {
            _chatService = chatService;
            _geminiService = geminiService;
            _openAiService = openAiService;
        }

        [HttpPost("sessions")]
        public async Task<IActionResult> CreateSession()
        {
            var session = await _chatService.CreateSessionAsync();
            return Ok(session);
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetAllSessions()
        {
            var sessions = await _chatService.GetAllSessionsAsync();
            return Ok(sessions);
        }

        [HttpGet("sessions/{sessionId}/messages")]
        public async Task<IActionResult> GetMessages(Guid sessionId)
        {
            var messages = await _chatService.GetMessageBySessionIdAsync(sessionId);
            return Ok(messages);
        }

        [HttpPost("sessions/{sessionId}/messages")]
        public async Task<IActionResult> SendMessage(Guid sessionId, [FromBody] SendMessageRequest request)
        {
            if (string.IsNullOrEmpty(request.Content))
                return BadRequest("Mesaj boş olamaz.");

           
            await _chatService.AddMessageAsync(sessionId, request.Content, "User", null);

           
            var conversationHistory = await _chatService.GetMessageBySessionIdAsync(sessionId);

            string aiResponse = "";
            string senderName = "";

            if (request.Model.StartsWith("gpt"))
            {
                try
                {
                    aiResponse = await _openAiService.GetResponseAsync(conversationHistory, request.Model);
                    senderName = "ChatGPT";
                }
                catch (Exception ex)
                {
                    return BadRequest($"OpenAI Hatası: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    aiResponse = await _geminiService.GetResponseAsync(conversationHistory, request.Model);
                    senderName = "Gemini";
                }
                catch (Exception ex)
                {
                    return BadRequest($"Gemini Hatası: {ex.Message}");
                }
            }
          
            await _chatService.AddMessageAsync(sessionId, aiResponse, senderName, request.Model);

            return Ok(new
            {
                Message = "Cevap alındı",
                UserContent = request.Content,
                AiContent = aiResponse
            });
        }
        [HttpDelete("sessions/{sessionId}")]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            await _chatService.DeleteSessionAsync(sessionId);
            return NoContent();
        }

        [HttpDelete("sessions/clear-all")]
        public async Task<IActionResult> ClearAll()
        {
            await _chatService.ClearAllSessionsAsync();
            return NoContent();
        }

        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            await _chatService.DeleteMessageAsync(messageId);
            return NoContent();
        }
    }

    public class SendMessageRequest
    {
        public string Content { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.5-flash";
    }
}