using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SolCentre.Models;

namespace SolCentre.Services
{
    public class EonetService : IEonetService
    {
        private readonly HttpClient _client;
        private readonly ConcurrentDictionary<string, object> _cache = new();

        public EonetService(IHttpClientFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _client = factory.CreateClient("Eonet");
        }

        public async Task<IReadOnlyList<EonetEvent>> GetEventsAsync(int limit = 50, int? days = null, string source = null, string status = null, CancellationToken ct = default)
        {
            var sb = new StringBuilder("events?");
            sb.Append($"limit={limit}");
            if (days.HasValue) sb.Append($"&days={days.Value}");
            if (!string.IsNullOrWhiteSpace(source)) sb.Append($"&source={Uri.EscapeDataString(source)}");
            if (!string.IsNullOrWhiteSpace(status)) sb.Append($"&status={Uri.EscapeDataString(status)}");

            var key = $"events_{limit}_{days}_{source}_{status}";
            if (_cache.TryGetValue(key, out var cached) && cached is List<EonetEvent> list)
                return list;

            try
            {
                var resp = await _client.GetFromJsonAsync<EonetModels>(sb.ToString(), ct);
                var events = resp?.Events ?? new List<EonetEvent>();
                _cache[key] = events;
                return events;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception)
            {
                return Array.Empty<EonetEvent>();
            }
        }

        public async Task<EonetEvent> GetEventByIdAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var key = $"event_{id}";
            if (_cache.TryGetValue(key, out var cached) && cached is EonetEvent ev) return ev;

            try
            {
                var e = await _client.GetFromJsonAsync<EonetEvent>($"events/{Uri.EscapeDataString(id)}", ct);
                if (e != null) _cache[key] = e;
                return e;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<IReadOnlyList<EonetEvent>> GetEventsAsync(int limit = 50, int? days = null, string source = null, string status = null)
        {
            return GetEventsAsync(limit, days, source, status, CancellationToken.None);
        }
    }
}
