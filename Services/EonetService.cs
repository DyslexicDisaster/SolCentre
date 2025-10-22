// Add file: Services/IEonetService.cs
using SolCentre.Models;

public interface IEonetService
{
    Task<List<EonetEvent>> GetRecentEventsAsync(int limit = 10, CancellationToken ct = default);
}

// Improved EonetService.cs (use IConfiguration & ILogger)
public class EonetService : IEonetService
{
    private readonly HttpClient _http;
    private readonly ILogger<EonetService> _logger;
    private readonly string _eventsPath; // e.g. "api/v3/events"

    public EonetService(HttpClient http, IConfiguration config, ILogger<EonetService> logger)
    {
        _http = http;
        _logger = logger;
        _eventsPath = config.GetValue<string>("Eonet:EventsPath") ?? "api/v3/events";
    }

    public async Task<List<EonetEvent>> GetRecentEventsAsync(int limit = 10, CancellationToken ct = default)
    {
        try
        {
            // Use relative URI so BaseAddress made in Program is used
            var url = $"{_eventsPath}?limit={limit}";
            var res = await _http.GetFromJsonAsync<EonetResponse>(url, ct);
            return res?.Events ?? new List<EonetEvent>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get EONET events");
            return new List<EonetEvent>();
        }
    }
}
