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
    public class ApodService : IApodService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public ApodService(IHttpClientFactory factory, IConfiguration config)
        {
            _client = factory.CreateClient("NasaApi");
            // read API key from config, fallback to DEMO_KEY (rate-limited)
            _apiKey = config["Nasa:ApiKey"] ?? "DEMO_KEY";
        }

        public async Task<ApodModel> GetApodAsync(DateTime? date = null, CancellationToken ct = default)
        {
            var url = $"planetary/apod?api_key={_apiKey}";
            if (date.HasValue)
            {
                // APOD expects date as YYYY-MM-DD
                var d = date.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                url += $"&date={Uri.EscapeDataString(d)}";
            }

            try
            {
                var res = await _client.GetFromJsonAsync<ApodModel>(url, ct);
                return res;
            }
            catch
            {
                return null;
            }
        }
    }
}
