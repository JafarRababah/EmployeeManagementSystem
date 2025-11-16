using Azure;
using EmployeesManagment.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class PayPalService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _cfg;
    private string BaseUrl => _cfg["PayPal:BaseUrl"];

    public PayPalService(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _cfg = cfg;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var clientId = _cfg["PayPal:ClientId"];
            var secret = _cfg["PayPal:Secret"];
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

            var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/v1/oauth2/token");
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["grant_type"] = "client_credentials" });

            var res = await _http.SendAsync(req);
            var json = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"خطأ من PayPal: {json}");
            }
            
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString();
        }
        catch (Exception ex)
        {
            throw new Exception("PayPal GetAccessToken failed: " + ex.Message, ex);
        }

    }


    public async Task<string> CreateOrder(decimal amount, string currency = "USD")
    {
        var token = await GetAccessTokenAsync();
        var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/v2/checkout/orders");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new
        {
            intent = "CAPTURE",
            purchase_units = new[] {
                new {
                    amount = new {
                        currency_code = currency,
                        value = amount.ToString("F2")
                    }
                }
            }
        };

        req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var res = await _http.SendAsync(req);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("id").GetString(); // orderID
    }

    public async Task<JsonElement> CaptureOrder(string orderId)
    {
        var token = await GetAccessTokenAsync();
        var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/v2/checkout/orders/{orderId}/capture");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        var res = await _http.SendAsync(req);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }
}
