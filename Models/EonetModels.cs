namespace SolCentre.Models
{
    public class EonetModels
    {
        public List<EonetEvent> Events { get; set; }
    }
    public class EonetEvent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Closed { get; set; }
        public List<EonetCategory> Categories { get; set; }
        public List<EonetGeometry> Geometries { get; set; }
    }
    public class EonetCategory
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
    public class EonetGeometry
    {
        public string Date { get; set; }
        public List<double> Coordinates { get; set; }
    }
}
