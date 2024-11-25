using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Wigle2Geo.ExpressionVisitors;
using Wigle2Geo.Models;
using Wigle2Geo.Models.GeoJson;

namespace Wigle2Geo.Services
{
    public class MinMaxRange<T>(T min, T max)
    {
        public T Min { get; } = min;
        public T Max { get; } = max;
    }

    public class NetworkSearchFilter
    {
        public string[] ssid;
        public string[] bssid;
        public string[] types;
        public string? capabilities;
        public MinMaxRange<int?> distance;
        public MinMaxRange<long?> duration;
        public MinMaxRange<int?> locations;
        public MinMaxRange<long?> time;
    }

    public class GeoJsonNetworksService(
        ILoggerFactory loggerFactory,
         WiGleBackupContext context,
         IVendorResolverService vendorResolverService
    )
    {
        const int CoordinatesAccuracy = 6;
        ILogger? logger = loggerFactory?.CreateLogger<GeoJsonNetworksService>();

        public IEnumerable<Feature> Get(NetworkSearchFilter filter)
        {
            logger?.LogInformation("Getting networks");
            FormattableString query = $"SELECT n.*, COUNT(*) as locations, max(time)-min(time) as duration, cast(2 * 6335 * 1000 \r\n        * asin(sqrt(\r\n            pow(sin((radians(max(lat)) - radians(min(lat))) / 2), 2)\r\n            + cos(radians(min(lat)))\r\n            * cos(radians(max(lat)))\r\n            * pow(sin((radians(max(lon)) - radians(min(lon))) / 2), 2)\r\n        )) AS INT) AS distance FROM network AS n LEFT JOIN location AS l ON \r\n\t\tn.bssid = l.bssid and \r\n\t\t\"time\" <> 0 and\r\n\t\t\"type\" = \"W\"\r\n\t\tgroup by n.bssid";

            //var result = 
            //_context.Database.SqlQuery<NetworkExt>(query).Select(network => new GeoJsonFeature()
            //{
            //    Geometry = new GeoJsonGeometryPoint(network.Bestlon, network.Bestlat),
            //    Id = network.Bssid,
            //    Properties = new Dictionary<string, object>
            //        {
            //            { "type",network.Type },
            //            { "bssid",network.Bssid },
            //            { "ssid",network.Ssid },
            //            { "capabilities",network.Capabilities },
            //            { "lasttime",network.Lasttime },
            //            { "lastlat",network.Lastlat },
            //            { "lastlon",network.Lastlon },
            //            { "bestlevel",network.Bestlevel },
            //            { "locations", network.Locations },
            //            { "distance", network.Distance },
            //        }
            //});
            Expression<Func<NetworkExt, bool>> query1 = (NetworkExt v) =>
            !(filter.ssid.Length > 0 && !filter.ssid.Any(s => EF.Functions.Like(v.Ssid, s))) &&
            !(filter.bssid.Length > 0 && !filter.bssid.Any(s => EF.Functions.Like(v.Bssid, s))) &&
            (filter.capabilities == null || EF.Functions.Like(v.Capabilities, filter.capabilities)) &&
            (filter.distance.Min == null || v.Distance > filter.distance.Min) &&
            (filter.distance.Max == null || v.Distance < filter.distance.Max) &&
            (filter.locations.Min == null || v.Locations > filter.locations.Min) &&
            (filter.locations.Max == null || v.Locations < filter.locations.Max) &&
            (filter.time.Min == null || v.Lasttime > filter.time.Min * 1000) &&
            (filter.time.Max == null || v.Lasttime < filter.time.Max * 1000) &&
            (filter.duration.Min == null || v.Duration > filter.duration.Min) &&
            (filter.duration.Max == null || v.Duration < filter.duration.Max) &&
            (!filter.types.Any() || filter.types.Contains(v.Type)
            );

            IQueryable<Feature> result = context.NetworksExt.Where<NetworkExt>(SqliteExpressionOptimizer.Transform(query1))
              .Select(
                (network) =>
                //.Select(network => 
                new Feature()
                {
                    Geometry = new GeometryPoint(Math.Round(network.Bestlon, CoordinatesAccuracy), Math.Round(network.Bestlat, CoordinatesAccuracy)),
                    Id = network.Bssid,

                    Properties = new Dictionary<string, object?>
                    {
                        { "type",network.Type },
                        { "bssid",network.Bssid },
                        { "ssid",network.Ssid },
                        { "cap",network.Capabilities },
                        { "lasttime",network.Lasttime },
                        { "lastlat", Math.Round(network.Lastlat, CoordinatesAccuracy) },
                        { "lastlon", Math.Round(network.Lastlon, CoordinatesAccuracy) },
                        { "level",network.Bestlevel },
                        { "loc", network.Locations },
                        { "dist", network.Distance },
                        { "dur", network.Duration },
                        { "vendor", vendorResolverService.Get(network.Bssid) },
                        //{ "distance", (int?)Distance(
                        //    Math.Round(locations.Min(v=>v.Lat),CoordinatesAccuracy),
                        //    Math.Round(locations.Min(v=>v.Lon),CoordinatesAccuracy), 
                        //    Math.Round(locations.Max(v=>v.Lat),CoordinatesAccuracy),
                        //    Math.Round(locations.Max(v=>v.Lon),CoordinatesAccuracy)
                        //)},
                      //  { "distance", DistanceFromA(Math.Pow(Math.Sin((locations.Max(v => v.Lat) - locations.Min(v => v.Lat)) * PI180 / 2), 2) + Math.Cos(locations.Min(v => v.Lat) * PI180) * Math.Cos(locations.Max(v => v.Lat) * PI180) * Math.Pow(Math.Sin((locations.Max(v => v.Lon) - locations.Min(v => v.Lon)) * PI180 / 2), 2))                        },
                    }
                });


            //var result = _context.Networks.Where(v =>
            //(ssid == null || EF.Functions.Like(v.Ssid, ssid)) &&
            //(bssid == null || EF.Functions.Like(v.Bssid, bssid)) &&
            //(capabilities == null || EF.Functions.Like(v.Capabilities, capabilities)) &&
            //(type == null || types.Contains(v.Type)))
            //    .GroupJoin(_context.Locations.Where(l => l.Time != 0),
            //    network => network.Bssid,
            //    location => location.Bssid,
            //    (network, locations) =>
            //    //.Select(network => 
            //    new GeoJsonFeature()
            //    {
            //        Geometry = new GeoJsonGeometryPoint(Math.Round(network.Bestlon, CoordinatesAccuracy), Math.Round(network.Bestlat, CoordinatesAccuracy)),
            //        Id = network.Bssid,

            //        Properties = new Dictionary<string, object>
            //        {
            //            { "type",network.Type },
            //            { "bssid",network.Bssid },
            //            { "ssid",network.Ssid },
            //            { "capabilities",network.Capabilities },
            //            { "lasttime",network.Lasttime },
            //            { "lastlat", Math.Round(network.Lastlat, CoordinatesAccuracy) },
            //            { "lastlon", Math.Round(network.Lastlon, CoordinatesAccuracy) },
            //            { "bestlevel",network.Bestlevel },
            //            { "locations", locations.Count() },
            //            //{ "distance", (int?)Distance(
            //            //    Math.Round(locations.Min(v=>v.Lat),CoordinatesAccuracy),
            //            //    Math.Round(locations.Min(v=>v.Lon),CoordinatesAccuracy), 
            //            //    Math.Round(locations.Max(v=>v.Lat),CoordinatesAccuracy),
            //            //    Math.Round(locations.Max(v=>v.Lon),CoordinatesAccuracy)
            //            //)},
            //          //  { "distance", DistanceFromA(Math.Pow(Math.Sin((locations.Max(v => v.Lat) - locations.Min(v => v.Lat)) * PI180 / 2), 2) + Math.Cos(locations.Min(v => v.Lat) * PI180) * Math.Cos(locations.Max(v => v.Lat) * PI180) * Math.Pow(Math.Sin((locations.Max(v => v.Lon) - locations.Min(v => v.Lon)) * PI180 / 2), 2))                        },

            //        { "distance", DistanceFromA(
            //            Math.Pow(Math.Sin(double.DegreesToRadians(locations.Max(v => v.Lat) - locations.Min(v => v.Lat)) / 2), 2)+
            //            Math.Cos(double.DegreesToRadians(locations.Min(v => v.Lat))) * Math.Cos(double.DegreesToRadians(locations.Max(v => v.Lat))) *
            //            Math.Pow(Math.Sin(double.DegreesToRadians(locations.Max(v => v.Lon) - locations.Min(v => v.Lon)) / 2), 2)
            //            )                        },

            //        }
            //    });
            foreach (var feature in result)
            {
                yield return feature;
            }
            logger?.LogInformation("Getting networks DONE");
            var str = result.ToQueryString();
        }


        private static double? Distance(double? lat1, double? lon1, double? lat2, double? lon2)
        {
            if (lat1 == null || lat2 == null || lon1 == null || lon2 == null)
                return null;

            // generally used geo measurement function
            var dLatS2 = Math.Pow(Math.Sin(double.DegreesToRadians(lat2.Value - lat1.Value) / 2), 2);
            var dLonS2 = Math.Pow(Math.Sin(double.DegreesToRadians(lon2.Value - lon1.Value) / 2), 2);
            var a = dLatS2 + Math.Cos(double.DegreesToRadians(lat1.Value)) * Math.Cos(double.DegreesToRadians(lat2.Value)) * dLonS2;
            return DistanceFromA(a);
        }

        private static int? DistanceFromA(double? a)
        {
            if (a == null)
                return null;
            return (int?)(6378.137 * 2 * Math.Atan2(Math.Sqrt(a.Value), Math.Sqrt(1 - a.Value)) * 1000);
        }

        private static double? Distance(double? lat1, double? lat2, double? dLonS2)
        {
            if (lat1 == null || lat2 == null || dLonS2 == null)
                return null;

            // generally used geo measurement function
            // const double PI180 = Math.PI / 180;
            var dLatS2 = Math.Pow(Math.Sin(double.DegreesToRadians(lat2.Value - lat1.Value) / 2), 2);
            //var dLonS2 = Math.Pow(Math.Sin((lon2.Value - lon1.Value) * PI180 / 2), 2);
            var a = dLatS2 + Math.Cos(double.DegreesToRadians(lat1.Value)) * Math.Cos(double.DegreesToRadians(lat2.Value)) * dLonS2.Value;
            return DistanceFromA(a);
        }
    }
}
