namespace Wigle2Geo.Models.GeoJson
{
    public class Feature
    {
        public string Type => "feature";
        public string Id { get; set; }
        public GeometryPoint Geometry { get; set; }
        public Dictionary<string, object?> Properties { get; set; }
    }
}
