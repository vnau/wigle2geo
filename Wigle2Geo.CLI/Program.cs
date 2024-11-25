using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Text.Json;
using Wigle2Geo.Models;
using Wigle2Geo.Models.GeoJson;
using Wigle2Geo.Services;

namespace WiGle2Geo.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            // Create the root command
            var rootCommand = new RootCommand("WiGle2Geo CLI - A tool for retrieving GeoJson data from WiGle backup");
            // Define options for the 'network' command
            var sourceOption = new Option<string?>("--source", "Path to the WiGle backup database") { IsRequired = true, ArgumentHelpName = "path to wigle backup" };
            var ssidOption = new Option<string?>("--ssid", "Optional SSID filter");
            var bssidOption = new Option<string?>("--bssid", "Optional BSSID filter");
            var typeOption = new Option<string?>("--type", "Optional type filter, use | for multiple types");
            var capabilitiesOption = new Option<string?>("--capabilities", "Optional capabilities filter");
            var distanceGtOption = new Option<int?>("--distanceGt", "Optional filter for distance greater than") { ArgumentHelpName = "distance in meters" };
            var distanceLtOption = new Option<int?>("--distanceLt", "Optional filter for distance less than") { ArgumentHelpName = "distance in meters" };
            var locationsGtOption = new Option<int?>("--locationsGt", "Optional filter for locations greater than") { ArgumentHelpName = "number of points" };
            var locationsLtOption = new Option<int?>("--locationsLt", "Optional filter for locations less than") { ArgumentHelpName = "number of points" };

            // Create 'network' command
            var networkCommand = new Command("network", "Get networks based on optional filters")
            {
                ssidOption,
                bssidOption,
                typeOption,
                capabilitiesOption,
                distanceGtOption,
                distanceLtOption,
                locationsGtOption,
                locationsLtOption
            };

            // Use SetHandler to bind options directly to the handler function
            networkCommand.SetHandler(
                (string? ssid, string? bssid, string? type, string? capabilities, int? distanceGt, int? distanceLt, int? locationsGt, int? locationsLt) =>
                {
                    HandleNetwork(ssid, bssid, type, capabilities, distanceGt, distanceLt, locationsGt, locationsLt);
                },
                ssidOption, bssidOption, typeOption, capabilitiesOption, distanceGtOption, distanceLtOption, locationsGtOption, locationsLtOption
            );

            // Define option for the 'location' command
            var locationBssidOption = new Option<string>("--bssid", "BSSID value for which to get location data") { IsRequired = true };

            // Create 'location' command
            var locationCommand = new Command("location", "Get locations for a specified BSSID")
            {
                sourceOption,
                locationBssidOption
            };

            // Use SetHandler to bind options directly to the handler function
            locationCommand.SetHandler(
                (string? bssid) =>
                {
                    HandleLocation(bssid);
                },
                locationBssidOption
            );

            // Add subcommands to the root command
            rootCommand.AddCommand(networkCommand);
            rootCommand.AddCommand(locationCommand);

            // Invoke the root command with the passed arguments
            return rootCommand.Invoke(args);
        }

        // Handle 'network' action logic
        private static void HandleNetwork(string? ssid, string? bssid, string? type, string? capabilities, int? distanceGt, int? distanceLt, int? locationsGt, int? locationsLt)
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(b => { b.LogToStandardErrorThreshold = LogLevel.Trace; b.TimestampFormat = "dd.MM.yyyy HH:mm:ss.ff "; }); });

            var context = new WiGleBackupContext("..\\backup.sqlite", loggerFactory);
            //var ouiSource = new OuiSourceService(loggerFactory, "https://standards-oui.ieee.org/oui/oui.txt");
            var ouiSource = new OuiSourceService(loggerFactory, "..\\oui.txt");
            var resolver = new VendorResolverService(ouiSource, loggerFactory);
            var databaseService = new DatabaseService(loggerFactory, context);
            Task.WaitAll(resolver.StartingAsync(CancellationToken.None), databaseService.StartingAsync(CancellationToken.None));
            var networkService = new GeoJsonNetworksService(loggerFactory, context, resolver);
            var result = networkService.Get(
                new NetworkSearchFilter()
                {
                    ssid = ssid?.Split("|") ?? [],
                    bssid = bssid?.Split("|") ?? [],
                    types = type?.Split("|") ?? [],
                    capabilities = capabilities,
                    distance = new MinMaxRange<int?>(distanceGt, distanceLt),
                    locations = new MinMaxRange<int?>(null, null),
                    time = new MinMaxRange<long?>(null, null)
                }
                );
            // Console.WriteLine(JsonSerializer.Serialize(new FeatureCollection(result), new JsonSerializerOptions() { WriteIndented = true }));
            foreach (var network in result)
                //{
                //    //AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(network, new JsonSerializerOptions() { WriteIndented = true })));
                //    //AnsiConsole.Write(",\r\n");
                Console.WriteLine(JsonSerializer.Serialize(network, new JsonSerializerOptions() { WriteIndented = true }));
            //}
        }

        // Handle 'location' action logic
        private static void HandleLocation(string? bssid)
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var context = new WiGleBackupContext("..\\backup.sqlite");
            var ouiSource = new OuiSourceService(loggerFactory);
            var resolver = new VendorResolverService(ouiSource, loggerFactory);
            var locationService = new GeoJsonLocationsService(loggerFactory, context);

            // Simulating a service call
            //var locationService = new GeoJsonLocationsService();
            var result = locationService.Get(bssid);
            Console.WriteLine(JsonSerializer.Serialize(new FeatureCollection(result), new JsonSerializerOptions() { WriteIndented = true }));
            //foreach (var network in result)
            //{
            //    //AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(network, new JsonSerializerOptions() { WriteIndented = true })));
            //    //AnsiConsole.Write(",\r\n");
            //    Console.WriteLine(JsonSerializer.Serialize(network, new JsonSerializerOptions() { WriteIndented = true }));
            //}
        }
    }
}
