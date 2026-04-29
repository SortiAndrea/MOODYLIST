using System.Text.Json;
using System.Text;
using MOODYLIST.Model;

namespace MOODYLIST.Services
{
    public class AIChat
    {
        private  HttpClient _httpClient;
        private IConfiguration _configuration;

        public AIChat(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Canzone>> GeneratePlaylistAsync(string userInput)
        {
            var apikey = _configuration["Gemini:ApiKey"];
            var requestBody = new
            {
                contents = new[]
                {
            new {
                parts = new[]
                {
                    new {
                        text = $"In base a questo mood: {userInput}, genera 5 canzoni. " +
                               $"Formato: Titolo - Artista, una per riga"
                    }
                }
            }
        }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={apikey}",
                content);

            var result = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(result);

            // 🔴 Se Gemini restituisce errore
            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                return new List<Canzone>();
            }

            // 🔴 Se manca candidates
            if (!doc.RootElement.TryGetProperty("candidates", out var candidates))
            {
                return new List<Canzone>();
            }

            // 🔴 Se array vuoto
            if (candidates.GetArrayLength() == 0)
            {
                return new List<Canzone>();
            }

            // 🔴 Se manca content
            if (!candidates[0].TryGetProperty("content", out var contentJson))
            {
                return new List<Canzone>();
            }

            // 🔴 Se manca parts
            if (!contentJson.TryGetProperty("parts", out var parts))
            {
                return new List<Canzone>();
            }

            // 🔴 Se parts vuoto
            if (parts.GetArrayLength() == 0)
            {
                return new List<Canzone>();
            }

            // 🔴 Prendi il testo
            var text = parts[0].GetProperty("text").GetString();

            // 🔴 Se testo vuoto
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<Canzone>();
            }

            var songs = new List<Canzone>();

            foreach (var line in text.Split('\n'))
            {
                if (!line.Contains("-")) continue;

                var partsLine = line.Split('-');
                if (partsLine.Length < 2) continue;

                songs.Add(new Canzone
                {
                    Title = partsLine[0].Trim(),
                    Artist = partsLine[1].Trim()
                });
            }

            return songs;
        }
    }
}
