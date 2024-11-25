using Wigle2Geo.Models.OUI;

namespace Wigle2Geo.Services
{
    public interface IOuiSourceService
    {
        IAsyncEnumerable<OUIRecord> Get();
    }
}