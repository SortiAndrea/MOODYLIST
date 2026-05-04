using System.Text.Json;
using System.Text;
using MOODYLIST.Model;

namespace MOODYLIST.Services
{
    public class AIChat
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIChat(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Canzone>> GeneratePlaylistAsync(string userInput)
        {
            var apiKey = _configuration["Gemini:ApiKey"];

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"In base a questo mood: {userInput}, genera 5 canzoni. Formato: Titolo - Artista, una per riga" }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={apiKey}",
                content);

            var result = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(result);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                return new List<Canzone>();

            var text = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(text))
                return new List<Canzone>();

            var songs = new List<Canzone>();

            foreach (var line in text.Split('\n'))
            {
                var parts = line.Split('-');
                if (parts.Length < 2) continue;

                songs.Add(new Canzone
                {
                    Title = parts[0].Trim(),
                    Artist = parts[1].Trim()
                });
            }

            return songs;
        }
    }
}
