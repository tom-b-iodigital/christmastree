using ChristmasPoem.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChristmasPoem.Services.Implementations
{
    public class AIService : IAIService
    {
        private HttpClient _client;
        private JsonSerializerOptions _serializerOptions;

        public AIService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "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");
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<string> GetGreetingAsync()
        {
            var request = new AIRequest()
            {
                Prompt = $"Present yourself as a christmas tree in a funny way and ask the user to say a word that will be used to make a poem.",
            };

            Uri uri = new Uri("https://api.openai.com/v1/completions");
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync<AIRequest>(uri, request);
                if (response.IsSuccessStatusCode)
                {
                    AIResponse content = await response.Content.ReadFromJsonAsync<AIResponse>();

                    if (content.Choices.Any())
                    {
                        return content.Choices.First().Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return "";
        }

        public async Task<string> GetPoemAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                keyword = "marketing";
            }

            var request = new AIRequest()
            {
                Prompt = $"Write me a short christmas poem about {keyword}, make it rhyme",
            };

            Uri uri = new Uri("https://api.openai.com/v1/completions");
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync<AIRequest>(uri, request);
                if (response.IsSuccessStatusCode)
                {
                    AIResponse content = await response.Content.ReadFromJsonAsync<AIResponse>();

                    if (content.Choices.Any()) 
                    {
                        return content.Choices.First().Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return "";
        }
    }
}
