namespace Wigle2Geo.Models.GeoJson
{
    public class FeatureCollection(IEnumerable<Feature> features)
    {
        public string Type => "FeatureCollection";
        public IEnumerable<Feature> Features { get; } = features;
    }
}
