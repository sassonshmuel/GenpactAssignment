using System.Text.Json;

namespace GenpactAssignment.Infra.Api;

public abstract class ApiClient : IAsyncDisposable
{
    private readonly HttpClient _httpClient;

    protected ApiClient(HttpClient? client = null)
    {
        _httpClient = client ?? new HttpClient();
        // 👇 Wikipedia API requires a valid User-Agent
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GenpactAssignmentBot/1.0 (contact: yourname@example.com)");
    }

    protected async Task<JsonDocument> GetJsonAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);

        // Optionally retry if Wikipedia temporarily rate-limits
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error {response.StatusCode}: {content}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient.Dispose();
        await Task.CompletedTask;
    }
}