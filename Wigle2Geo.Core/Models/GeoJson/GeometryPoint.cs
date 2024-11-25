namespace Wigle2Geo.Models.GeoJson
{
    public class GeometryPoint(double lon, double lat)
    {
        public string Type => "Point";
        public double[] Coordinates { get; } = [lon, lat];
    }
}
