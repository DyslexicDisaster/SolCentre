using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolCentre.Models
{
    //Earth Observatory Natural Event Tracker (EONET) 
    public class EonetModels
    {
        [JsonPropertyName("events")]
        public List<EonetEvent> Events { get; set; } = new();
    }


    public class EonetEvent
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("link")] public string Link { get; set; }
        [JsonPropertyName("categories")] public List<EonetCategory> Categories { get; set; } = new();
        [JsonPropertyName("sources")] public List<EonetSource> Sources { get; set; } = new();
        [JsonPropertyName("geometries")] public List<EonetGeometry> Geometries { get; set; } = new();
        [JsonPropertyName("closed")] public DateTimeOffset? Closed { get; set; }
    }

    public class EonetCategory
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("link")] public string Link { get; set; }
    }

    public class EonetSource
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("source")] public string Source { get; set; }
        [JsonPropertyName("link")] public string Link { get; set; }   
    }

    public class EonetGeometry
    {
        [JsonPropertyName("date")]
        public DateTimeOffset Date { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } 

        [JsonPropertyName("coordinates")]
        public JsonElement Coordinates { get; set; }
    }
}
