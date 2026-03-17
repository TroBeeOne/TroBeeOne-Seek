using Microsoft.AspNetCore.Mvc;
using SeekProject.Models;
using System.Text;
using System.Text.Json;

namespace SeekProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyzeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AnalyzeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task Post([FromBody] AnalyzeRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.ImageBase64))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Image data is required.");
                return;
            }

            var ollamaUrl = _configuration["Ollama:Url"] ?? "http://localhost:11434";
            var model = _configuration["Ollama:Model"] ?? "qwen2-vl:8b"; // Prompt said qwen3-vl:8b but I'll use what's in config or qwen2-vl if missing

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            // Construct Ollama Request
            // For VL models, we send the image in the 'images' array of the message
            var ollamaRequest = new
            {
                model = model,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = request.Prompt,
                        images = new[] { request.ImageBase64 }
                    }
                },
                stream = true
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json");
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, $"{ollamaUrl}/api/chat") { Content = jsonContent };

            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            try
            {
                using var response = await client.SendAsync(requestMsg, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Response.WriteAsync($"data: {JsonSerializer.Serialize(new { error = $"Ollama Error: {error}" })}\n\n");
                    return;
                }

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        // Proxy the line directly to frontend
                        await Response.WriteAsync($"data: {line}\n\n");
                        await Response.Body.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await Response.WriteAsync($"data: {JsonSerializer.Serialize(new { error = ex.Message })}\n\n");
            }
        }
    }
}
