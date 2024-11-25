using System.ComponentModel.DataAnnotations;

namespace Wigle2Geo.Models;

public partial class Network
{
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
}
