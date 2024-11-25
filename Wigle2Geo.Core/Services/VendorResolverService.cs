using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Wigle2Geo.Services
{
    public class VendorResolverService : IVendorResolverService, IHostedLifecycleService
    {
        private Dictionary<string, string> ouiRecords = null;
        private readonly IOuiSourceService? sourceService;
        private ILogger? logger = null;


        public VendorResolverService(IOuiSourceService sourceService, ILoggerFactory loggerFactory)
        {
            this.sourceService = sourceService;
            logger = loggerFactory?.CreateLogger<VendorResolverService>();
        }

        public string? Get(string mac)
        {
            var key = mac.Replace(":", "").Substring(0, 6).ToUpper();
            return ouiRecords.ContainsKey(key) ? ouiRecords[key] : null;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //  throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            logger?.LogInformation("Loading OUI");
            var records = new Dictionary<string, string>();
            await foreach (var record in sourceService.Get())
            {
                var key = record.Base16.ToUpper();
                records[key] = string.Intern(record.Organization);
            }
            this.ouiRecords = records;
            logger?.LogInformation("Loading OUI DONE");
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
