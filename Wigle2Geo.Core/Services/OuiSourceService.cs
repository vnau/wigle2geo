using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Wigle2Geo.Models.OUI;

namespace Wigle2Geo.Services
{
    public partial class OuiSourceService : IOuiSourceService
    {
        private readonly string ouiFileSource;
        private readonly ILogger logger;

        public OuiSourceService(ILoggerFactory loggerFactory, string ouiFileSource)
        {
            this.ouiFileSource = ouiFileSource;
            this.logger = loggerFactory.CreateLogger<OuiSourceService>();
        }

        //public OuiSourceService(ILoggerFactory loggerFactory) : this(loggerFactory, "oui.txt")
        public OuiSourceService(ILoggerFactory loggerFactory) : this(loggerFactory, "https://standards-oui.ieee.org/oui/oui.txt")
        {
        }

        public async IAsyncEnumerable<OUIRecord> Get()
        {
            Stream? stream = null;

            try
            {
                //using var scope = logger.BeginScope("Loading log from {source}", ouiFileSource);
                logger.LogInformation("Loading OUI data from {source}", ouiFileSource);
                // Determine if the input is a URL or a file path.
                if (Uri.TryCreate(ouiFileSource, UriKind.Absolute, out Uri uri) && uri.Scheme.StartsWith("http"))
                {
                    // If the input is a URL, download the file using HttpClient.
                    using HttpClient client = new HttpClient();
                    stream = await client.GetStreamAsync(ouiFileSource);
                }
                else
                {
                    // Otherwise, treat the input as a local file path and open a FileStream.
                    stream = new FileStream(ouiFileSource, FileMode.Open, FileAccess.Read, FileShare.Read);
                }

                // Parse the file content from the stream.
                await foreach (var record in ParseStream(stream))
                {
                    yield return record;
                }
                logger.LogInformation("Loading OUI data from {source} done", ouiFileSource);
            }
            finally
            {
                // Ensure the stream is closed when done.
                stream?.Dispose();
            }
        }

        // Function to parse OUI file.
        private static async IAsyncEnumerable<OUIRecord> ParseStream(Stream ouiStream)
        {
            OUIRecord? currentRecord = null;

            using var reader = new StreamReader(ouiStream);

            // Regex patterns to match different parts.
            var macPattern = MacPatternRegex();
            var base16Pattern = Base16PatternRegex();
            var addressPattern = AddressPatternRegex();

            // Read line by line and extract information.
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var macMatch = macPattern.Match(line);
                var base16Match = base16Pattern.Match(line);
                var addressMatch = addressPattern.Match(line);

                if (macMatch.Success)
                {
                    // Create a new record for a new MAC address.
                    if (currentRecord != null)
                    {
                        yield return currentRecord;
                    }

                    currentRecord = new OUIRecord
                    {
                        MacAddress = macMatch.Groups[1].Value,
                        Organization = macMatch.Groups[2].Value.Trim()
                    };
                }
                else if (base16Match.Success && currentRecord != null)
                {
                    // Add base-16 identifier to the existing record.
                    currentRecord.Base16 = base16Match.Groups[1].Value;
                }
                else if (addressMatch.Success && currentRecord != null)
                {
                    // Append address information to the existing record.
                    currentRecord.Address += addressMatch.Groups[1].Value.Trim() + " ";
                }
            }

            // Add the last record if exists.
            if (currentRecord != null)
            {
                yield return currentRecord;
            }
        }

        [GeneratedRegex(@"^([0-9A-Fa-f]{2}-[0-9A-Fa-f]{2}-[0-9A-Fa-f]{2})\s+\(hex\)\s+(.*)$")]
        private static partial Regex MacPatternRegex();
        [GeneratedRegex(@"^([0-9A-Fa-f]{6})\s+\(base 16\)\s+(.*)$")]
        private static partial Regex Base16PatternRegex();
        [GeneratedRegex(@"^\s+(.*)$")]
        private static partial Regex AddressPatternRegex();
    }

}
