using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiService(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpClient = httpFactory.CreateClient();
        _apiKey = config["OpenAI:ApiKey"];
    }

    public async Task<string> GetChatResponseAsync(string prompt)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var body = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] {
                new { role = "system", content = "You are an expert and customer support in a SkinCare system, your name is Lumia. You can speak both Vietnamese/English. If your prompt is english, answer english, otherwise, speak vietnamese." },
                new { role = "user", content = prompt }
            },
            max_tokens = 512,
            temperature = 0.7,
            stream = false
        };
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content?.Trim();
    }
}
