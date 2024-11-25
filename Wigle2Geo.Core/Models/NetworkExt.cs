using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wigle2Geo.Models
{
    [Table("network_temp")]
    public class NetworkExt //: Network
    {
        [Key]
        public string Bssid { get; set; } = null!;

        public string Ssid { get; set; } = null!;

        public int Frequency { get; set; }

        public string Capabilities { get; set; } = null!;

        public long Lasttime { get; set; }

        public double Lastlat { get; set; }

        public double Lastlon { get; set; }

        public string Type { get; set; } = null!;

        public int Bestlevel { get; set; }

        public double Bestlat { get; set; }

        public double Bestlon { get; set; }
        public double? Distance { get; set; }
        public long? Duration { get; set; }
        public int? Locations { get; set; }
    }
}
