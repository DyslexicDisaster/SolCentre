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
    // Service that talks to the EONET API
    public class EonetService : IEonetService
    {
        // Named HttpClient used to make requests to the EONET API
        private readonly HttpClient _client;

        // Simple thread-safe in-memory cache to avoid repeated network calls
        private readonly ConcurrentDictionary<string, object> _cache = new();

        // Constructor obtains an HttpClient from the factory (named "Eonet")
        public EonetService(IHttpClientFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _client = factory.CreateClient("Eonet");
        }

        // Fetch a list of events with optional filters and cancellation support
        public async Task<IReadOnlyList<EonetEvent>> GetEventsAsync(int limit = 50, int? days = null, string source = null, string status = null, CancellationToken ct = default)
        {
            // Build the relative query string for the events endpoint
            var sb = new StringBuilder("events?");
            sb.Append($"limit={limit}");
            if (days.HasValue) sb.Append($"&days={days.Value}");
            if (!string.IsNullOrWhiteSpace(source)) sb.Append($"&source={Uri.EscapeDataString(source)}");
            if (!string.IsNullOrWhiteSpace(status)) sb.Append($"&status={Uri.EscapeDataString(status)}");

            // Create a cache key based on the query parameters
            var key = $"events_{limit}_{days}_{source}_{status}";

            // Return cached result if available to save network roundtrips
            if (_cache.TryGetValue(key, out var cached) && cached is List<EonetEvent> list)
                return list;

            try
            {
                // Call the API and deserialize JSON into the wrapper model
                var resp = await _client.GetFromJsonAsync<EonetModels>(sb.ToString(), ct);

                // Extract events list (or empty list if response is null)
                var events = resp?.Events ?? new List<EonetEvent>();

                _cache[key] = events;

                return events;
            }
            catch (OperationCanceledException) { throw; } 
            catch (Exception)
            {
                // On error, return an empty collection so callers can continue safely
                return Array.Empty<EonetEvent>();
            }
        }

        // Fetch a single event by id, using cache when possible
        public async Task<EonetEvent> GetEventByIdAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            // Cache key for a single event
            var key = $"event_{id}";

            // Return cached event if present
            if (_cache.TryGetValue(key, out var cached) && cached is EonetEvent ev) return ev;

            try
            {
                // Request the specific event endpoint and deserialize
                var e = await _client.GetFromJsonAsync<EonetEvent>($"events/{Uri.EscapeDataString(id)}", ct);

                // Cache the result if found.
                if (e != null) _cache[key] = e;

                return e;
            }
            catch (OperationCanceledException) { throw; } 
            catch (Exception)
            {
                // On error, return null to indicate failure to fetch details
                return null;
            }
        }

        // Convenience overload that forwards to the cancellation-aware version with no cancellation token
        public Task<IReadOnlyList<EonetEvent>> GetEventsAsync(int limit = 50, int? days = null, string source = null, string status = null)
        {
            return GetEventsAsync(limit, days, source, status, CancellationToken.None);
        }
    }
}
