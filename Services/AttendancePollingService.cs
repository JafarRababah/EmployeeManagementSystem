using EmployeesManagment.Controllers.Api;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class AttendancePollingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AttendancePollingService> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public AttendancePollingService(HttpClient httpClient, ILogger<AttendancePollingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // إعداد Polly Retry Policy
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
                onRetry: (response, timespan, retryCount, context) =>
                {
                    _logger.LogWarning($"Retry {retryCount} after {timespan.TotalSeconds}s due to {response.Result.StatusCode}");
                });
    }

    public async Task PollAttendanceAsync(AttendanceWebhookDto attendance)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync("api/AttendanceWebhook", attendance);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Attendance posted successfully");
                }
                else
                {
                    _logger.LogError($"Failed to post attendance: {response.StatusCode}");
                }

                return response;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while polling attendance");
        }
    }
}
