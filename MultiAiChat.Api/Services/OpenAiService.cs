using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MultiAiChat.Api.Models;

namespace MultiAiChat.Api.Services
{
    public class OpenAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AiSettings:OpenAiApiKey"];
        }

        public async Task<string> GetResponseAsync(List<ChatMessage> conversationHistory, string model)
        {
            var requestUrl = "https://api.openai.com/v1/chat/completions";

            var messages = conversationHistory.Select(msg => new
            {
                role = msg.Sender == "User" ? "user" : "assistant",
                content = msg.Content
            }).ToList();

            var requestBody = new
            {
                model = model, 
                messages = messages
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI Hatası: {errorMsg}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var openAiResponse = JsonSerializer.Deserialize<OpenAiResponseRoot>(responseString);

            return openAiResponse?.Choices?[0]?.Message?.Content ?? "Cevap yok.";
        }
    }

    public class OpenAiResponseRoot
    {
        [JsonPropertyName("choices")]
        public List<OpenAiChoice>? Choices { get; set; }
    }

    public class OpenAiChoice
    {
        [JsonPropertyName("message")]
        public OpenAiMessage? Message { get; set; }
    }

    public class OpenAiMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}