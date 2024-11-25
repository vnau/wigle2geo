namespace Wigle2Geo.Models;

public partial class Route
{
    public int Id { get; set; }

    public int RunId { get; set; }

    public int WifiVisible { get; set; }

    public int CellVisible { get; set; }

    public int BtVisible { get; set; }

    public double Lat { get; set; }

    public double Lon { get; set; }

    public double Altitude { get; set; }

    public double Accuracy { get; set; }

    public byte[] Time { get; set; } = null!;
}
