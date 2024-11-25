namespace Wigle2Geo.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Bssid { get; set; } = null!;

    public int Level { get; set; }

    public double Lat { get; set; }

    public double Lon { get; set; }

    public double Altitude { get; set; }

    public double Accuracy { get; set; }

    public long Time { get; set; }

    public int External { get; set; }
}
