using SolCentre.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace SolCentre.Services
{
    public class EonetService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://eonet.gsfc.nasa.gov/api/v3/events";
        private const string ApiKey = "O028TvGhg1DWJ4uboQ4LijmfTgboh6iEw2JqWNjA"; 

        public EonetService(HttpClient http)
        {
            _httpClient = http;
        }

        public async Task<List<EonetEvent>> GetRecentEventsAsync(int limit = 10)
        {
            try
            {
                string url = $"{BaseUrl}?limit={limit}&api_key={ApiKey}";
                var response = await _httpClient.GetFromJsonAsync<EonetModels>(url);
                return response?.Events ?? new List<EonetEvent>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching EONET data: {ex.Message}");
                return new List<EonetEvent>();
            }
        }
    }

}
