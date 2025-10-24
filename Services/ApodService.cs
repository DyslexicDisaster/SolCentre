using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SolCentre.Models;

namespace SolCentre.Services
{
    // Service that fetches NASA's APOD (Astronomy Picture of the Day).
    public class ApodService : IApodService
    {
        // Named HttpClient for NASA API requests.
        private readonly HttpClient _client;

        // API key read from configuration (falls back to DEMO_KEY if not set).
        private readonly string _apiKey;

        // Constructor obtains a named HttpClient and reads API key from configuration.
        public ApodService(IHttpClientFactory factory, IConfiguration config)
        {
            _client = factory.CreateClient("NasaApi"); // use named client "NasaApi"
            _apiKey = config["Nasa:ApiKey"] ?? "DEMO_KEY"; // fallback to DEMO_KEY if missing
        }

        // Get APOD for a specific date or today's if date is null; supports cancellation.
        public async Task<ApodModel> GetApodAsync(DateTime? date = null, CancellationToken ct = default)
        {
            // Build the relative URL including the API key.
            var url = $"planetary/apod?api_key={_apiKey}";

            if (date.HasValue)
            {
                // APOD expects date format YYYY-MM-DD
                var d = date.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                url += $"&date={Uri.EscapeDataString(d)}";
            }

            try
            {
                // Send request and deserialize JSON to ApodModel.
                var res = await _client.GetFromJsonAsync<ApodModel>(url, ct);
                return res;
            }
            catch
            {
                // On any error return null 
                return null;
            }
        }
    }
}
