using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MultiAiChat.Api.Models;

namespace MultiAiChat.Api.Services
{
    public class GeminiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AiSettings:GeminiApiKey"];
        }

        public async Task<string> GetResponseAsync(List<ChatMessage> conversationHistory, string model)
        {
            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";

            var geminiContents = conversationHistory.Select(msg => new
            {
                role = msg.Sender == "User" ? "user" : "model",
                parts = new[]
                {
                    new { text = msg.Content }
                }
            }).ToList();

            var requestBody = new
            {
                contents = geminiContents
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Hatası: {errorMsg}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponseRoot>(responseString);

            return geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "Cevap yok.";
        }
    }

    public class GeminiResponseRoot
    {
        [JsonPropertyName("candidates")]
        public List<Candidate>? Candidates { get; set; }
    }
    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
    }
    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part>? Parts { get; set; }
    }
    public class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}