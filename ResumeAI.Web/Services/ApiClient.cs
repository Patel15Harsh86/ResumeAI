using System.Net.Http.Headers;
using System.Text.Json;

namespace ResumeAI.Web.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;

    public ApiClient(HttpClient httpClient, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
    }

    private void SetAuthHeader()
    {
        var token = _contextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<T?> PostFormAsync<T>(string endpoint, MultipartFormDataContent content)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsync(endpoint, content);
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync(endpoint);
        return response.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> PostRawAsync(string endpoint, object data)
    {
        SetAuthHeader();
        return await _httpClient.PostAsJsonAsync(endpoint, data);
    }
}