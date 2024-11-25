using Microsoft.Extensions.Logging;
using Wigle2Geo.Models;
using Wigle2Geo.Models.GeoJson;

namespace Wigle2Geo.Services
{
    public class GeoJsonLocationsService(
        ILoggerFactory loggerFactory,
         WiGleBackupContext context)
    {
        const int CoordinatesAccuracy = 6;
        public IEnumerable<Feature> Get(string bssid)
        {
            var network = bssid != null ? context.Networks.Where(v => bssid == v.Bssid).FirstOrDefault() ?? new Network() : null;
            var networkType = network?.Type;
            var networkSsid = network?.Ssid;
            var networkCaps = network?.Capabilities;

            return context.Locations.Where(v =>
            network == null || ((bssid == v.Bssid) && v.Time != 0)
            ).Select(v => new Feature()
            {
                Geometry = new GeometryPoint(Math.Round(v.Lon, CoordinatesAccuracy), Math.Round(v.Lat, CoordinatesAccuracy)),
                Id = v.Id.ToString(),
                Properties = new Dictionary<string, object?>
                    {
                        { "type",networkType },
                        { "bssid",v.Bssid },
                        { "altitude",Math.Round(v.Altitude,2) },
                        { "level",v.Level },
                        { "accuracy",Math.Round(v.Accuracy,2) },
                        { "time",v.Time },
                        { "ssid",networkSsid },
                        { "capabilities",networkCaps },
                    }
            });
        }
    }
}
