using Microsoft.AspNetCore.Mvc;
using Wigle2Geo.Core.DateTimeExtensions;
using Wigle2Geo.Models;
using Wigle2Geo.Models.GeoJson;
using Wigle2Geo.Services;

namespace Wigle2Geo.Web.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    //[EnableCors("CorsPolicy")]
    [Route("geo/[action]")]
    public class GeoJsonController : ControllerBase
    {
        private readonly ILogger<GeoJsonController> _logger;
        private readonly GeoJsonNetworksService _geoJsonNetworksService;
        private readonly GeoJsonLocationsService _geoJsonLocationsService;

        public GeoJsonController(
            ILogger<GeoJsonController> logger,
            GeoJsonNetworksService networkService,
            GeoJsonLocationsService geoJsonLocationsService)
        {
            _logger = logger;
            _geoJsonNetworksService = networkService;
            _geoJsonLocationsService = geoJsonLocationsService;
        }

        // application/geo+json
        [HttpGet]
        [ActionName("network")]
        public FeatureCollection GetNetworks(
            [FromQuery] string? ssid,
            [FromQuery] string? bssid,
            [FromQuery] string? type,
            [FromQuery] string? capabilities,
            [FromQuery] string? vendor,
            [FromQuery(Name = "distance[gt]")] int? distanceGt,
            [FromQuery(Name = "distance[lt]")] int? distanceLt,
            [FromQuery(Name = "locations[gt]")] int? locationsGt,
            [FromQuery(Name = "locations[lt]")] int? locationsLt,
            [FromQuery(Name = "time[gt]")] string? timeGt,
            [FromQuery(Name = "time[lt]")] string? timeLt,
            [FromQuery(Name = "duration[gt]")] string? durationGt,
            [FromQuery(Name = "duration[lt]")] string? durationLt
    )
        {
            static long? parseTime(string? datetime) => datetime == null ? null : DateTime.TryParse(datetime, out var time) ? (long?)time.ToUnixTimeSeconds() : long.Parse(datetime);

            var features = _geoJsonNetworksService.Get(
                new NetworkSearchFilter()
                {
                    ssid = ssid?.Split("|") ?? [],
                    bssid = bssid?.Split("|") ?? [],
                    types = type?.Split("|") ?? [],
                    capabilities = capabilities,
                    distance = new MinMaxRange<int?>(distanceGt, distanceLt),
                    locations = new MinMaxRange<int?>(locationsGt, locationsLt),
                    time = new MinMaxRange<long?>(parseTime(timeGt), parseTime(timeLt)),
                    duration = new MinMaxRange<long?>(parseTime(durationGt), parseTime(durationLt)),
                }
                );
            return new FeatureCollection(features);
        }

        [HttpGet]
        [ActionName("location")]
        public FeatureCollection GetLocations([FromQuery] string? bssid)
        {
            return new FeatureCollection(_geoJsonLocationsService.Get(bssid));
        }
    }
}
