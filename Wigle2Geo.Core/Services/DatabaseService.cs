using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wigle2Geo.Models;

namespace Wigle2Geo.Services
{
    public class DatabaseService : IHostedLifecycleService
    {
        private ILogger? logger;
        private WiGleBackupContext context;
        public DatabaseService(ILoggerFactory loggerFactory, WiGleBackupContext context)
        {
            logger = loggerFactory?.CreateLogger<DatabaseService>();
            this.context = context;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //logger?.LogInformation("Initialize DB");
            //await Task.Delay(3000);
            //logger?.LogInformation("Initialize DB done");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }


        public Task StartedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            var connectionString = context.Database.GetConnectionString();
            logger?.LogInformation("Migrate database {connectionString}", connectionString);
            await context.Database.MigrateAsync();
            logger?.LogInformation("Migrate database {connectionString} done", connectionString);
            logger?.LogInformation("Create a temporary table");
			// convert coordinates EPSG:4326 -> EPSG:3857
            await context.Database.ExecuteSqlAsync($"CREATE TEMPORARY TABLE IF NOT EXISTS network_temp AS\r\nSELECT n.*, COUNT(*) as locations, (MAX(time)-MIN(time))/1000 as duration, cast(2 * 6335 * 1000 \r\n        * asin(sqrt(\r\n            pow(sin((radians(max(lat)) - radians(min(lat))) / 2), 2)\r\n            + cos(radians(min(lat)))\r\n            * cos(radians(max(lat)))\r\n            * pow(sin((radians(max(lon)) - radians(min(lon))) / 2), 2)\r\n        )) AS INT) AS distance FROM network AS n LEFT JOIN location AS l ON \r\n\t\tn.bssid = l.bssid\r\n\t\tand \"time\" <> 0\r\n\t\tgroup by n.bssid");
            logger?.LogInformation("Create a temporary table done");
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
