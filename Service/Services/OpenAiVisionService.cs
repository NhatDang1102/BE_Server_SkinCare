using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OpenAiVisionService : IOpenAiVisionService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiVisionService(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpClient = httpFactory.CreateClient();
        _apiKey = config["OpenAI:ApiKey"];
    }

    public async Task<string> AnalyzeFaceAndSuggestRoutineAsync(byte[] imageBytes, List<string> productNames)
    {
        var base64Image = Convert.ToBase64String(imageBytes);

        var prompt = $@"Bạn là Lumia, AI tư vấn skincare chuyên nghiệp và giàu kinh nghiệm. 
1. Phân tích ảnh gương mặt người dùng (dựa trên ảnh) và chọn ra đúng 2 sản phẩm cho từng buổi (sáng, trưa, tối) chỉ trong danh sách sau:
{string.Join(", ", productNames)}
2. Sau đó, hãy **viết một lời khuyên skincare chi tiết, chuyên sâu bằng tiếng Việt**:
- Cá nhân hóa cho đúng tình trạng da mặt của người dùng trong ảnh (ví dụ: dầu, khô, mụn, thâm, lão hóa, v.v.).
- Lý giải tại sao bạn chọn các sản phẩm đó, hướng dẫn sử dụng từng sản phẩm trong routine đã chọn.
- Đưa ra các tips, lưu ý khi kết hợp hoặc trình tự dùng, cũng như nhấn mạnh các sai lầm thường gặp liên quan các sản phẩm trên.
- Ngắn gọn, súc tích, dễ áp dụng thực tế.

Hãy trả về ở đúng format JSON sau, không được sai, không được thừa thiếu:
{{
  ""morning"": [""product_name_1"", ""product_name_2""],
  ""noon"": [""product_name_1"", ""product_name_2""],
  ""night"": [""product_name_1"", ""product_name_2""],
  ""advice"": ""Một đoạn lời khuyên chuyên sâu như mô tả trên (bằng tiếng Việt)""
}}
Nếu ảnh không phải gương mặt người, trả về đúng 'Ảnh không hợp lệ'.";


        var body = new
        {
            model = "gpt-4.1-2025-04-14",
            messages = new object[]
           {
        new { role = "system", content = "You are Lumia, a professional skincare AI." },
        new
        {
            role = "user",
            content = new object[]
            {
                new { type = "text", text = prompt },
                new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
            }
        }
           },
            max_tokens = 600,
            temperature = 0.3
        };



        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
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
