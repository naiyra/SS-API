using System.Text.Json;
using System.Text;
namespace SS_API.Utils
{
    public class OpenRouterLabelGenerator
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "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"; // Replace this with your actual API key

        public OpenRouterLabelGenerator()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        }

        public async Task<string> GenerateLabelAsync(Dictionary<string, object> featureSummary)
        {
            // Convert the dictionary to a clean natural language list
            var profileText = string.Join(", ", featureSummary.Values.Select(v => v?.ToString()?.Trim()));

            var request = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new
            {
                role = "system",
                content = "You are a professional analyst. You generate short, clear, and professional segment titles for customer clusters based on summarized profile data."
            },
            new
            {
                role = "user",
                content = $"Given the following customer segment profile, generate a short and professional title that summarizes the group clearly. Focus on demographic traits, behavioral patterns, and preferences. Avoid slang or overly creative names. Return only the title.\n\n{profileText}"
            }
        }
            };

            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Raw OpenRouter response:\n{raw}");

                var result = JsonDocument.Parse(raw);
                var label = result.RootElement.GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return string.IsNullOrWhiteSpace(label) ? "General Customers" : label.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] OpenRouter failed: {ex.Message}");
                return "General Customers";
            }
        }

    }
}