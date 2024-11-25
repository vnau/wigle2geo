using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wigle2Geo.Models;

namespace Wigle2Geo.Web.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    //[EnableCors("CorsPolicy")]
    [Route("db/[action]")]
    public class WiGleBackupDbController : ControllerBase
    {
        private readonly ILogger<WiGleBackupDbController> _logger;
        private readonly WiGleBackupContext _context;

        public WiGleBackupDbController(
            ILogger<WiGleBackupDbController> logger,
            WiGleBackupContext context
          )
        {
            _logger = logger;
            _context = context;

        }

        [HttpGet]
        [ActionName("networks")]
        public async IAsyncEnumerable<Network> GetNetworks(
            [FromQuery] string? ssid,
            [FromQuery] string? bssid,
            [FromQuery] string? type,
            [FromQuery] string? capabilities
            )
        {
            var types = type == null ? [] : type.Split("|").ToArray();

            var networks = _context.Networks.Where(v =>
            (ssid == null || EF.Functions.Like(v.Ssid, ssid)) &&
            (bssid == null || EF.Functions.Like(v.Bssid, bssid)) &&
            (capabilities == null || EF.Functions.Like(v.Capabilities, capabilities)) &&
            (type == null || types.Contains(v.Type)));

            foreach (var network in networks)
            {
                yield return network;
            }
        }

        [HttpGet]
        [ActionName("locations")]
        public async IAsyncEnumerable<Location> GetLocations([FromQuery] string? bssid)
        {
            var locations = _context.Locations.Where(v =>
            (bssid == null || EF.Functions.Like(v.Bssid, bssid))
            );

            foreach (var location in locations)
            {
                yield return location;
            }
        }

        [HttpPost]
        [ActionName("upload")]
        public async Task<IActionResult> OnPostUploadAsync(IFormFile formFile)
        {
            long size = formFile.Length;

            if (size > 0)
            {
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }
            }


            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { size });
        }
    }
}
