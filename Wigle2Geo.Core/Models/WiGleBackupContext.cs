using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Wigle2Geo.Models;

public partial class WiGleBackupContext : DbContext
{
    private string connectionString = null;
    private ILogger? logger = null;

    public WiGleBackupContext(string connectionString, ILoggerFactory? loggerFactory = null)
    {
        logger = loggerFactory?.CreateLogger<WiGleBackupContext>();
        this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public WiGleBackupContext(DbContextOptions<WiGleBackupContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AndroidMetadatum> AndroidMetadata { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Network> Networks { get; set; }

    public virtual DbSet<NetworkExt> NetworksExt { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={connectionString}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AndroidMetadatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("android_metadata");

            entity.Property(e => e.Locale).HasColumnName("locale");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("location");
            entity.HasIndex(e => new { e.Bssid });
            entity.Property(e => e.Id).HasColumnName("_id");
            entity.Property(e => e.Accuracy)
                .HasColumnType("float")
                .HasColumnName("accuracy");
            entity.Property(e => e.Altitude)
                .HasColumnType("double")
                .HasColumnName("altitude");
            entity.Property(e => e.Bssid).HasColumnName("bssid");
            entity.Property(e => e.External).HasColumnName("external");
            entity.Property(e => e.Lat)
                .HasColumnType("double")
                .HasColumnName("lat");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Lon)
                .HasColumnType("double")
                .HasColumnName("lon");
            entity.Property(e => e.Time)
                .HasColumnType("long")
                .HasColumnName("time");
        });

        modelBuilder.Entity<Network>(entity =>
        {
            entity.HasKey(e => e.Bssid);

            entity.ToTable("network");

            entity.Property(e => e.Bssid).HasColumnName("bssid");
            entity.Property(e => e.Bestlat)
                .HasColumnType("double")
                .HasColumnName("bestlat");
            entity.Property(e => e.Bestlevel).HasColumnName("bestlevel");
            entity.Property(e => e.Bestlon)
                .HasColumnType("double")
                .HasColumnName("bestlon");
            entity.Property(e => e.Capabilities).HasColumnName("capabilities");
            entity.Property(e => e.Frequency)
                .HasColumnType("INT")
                .HasColumnName("frequency");
            entity.Property(e => e.Lastlat)
                .HasColumnType("double")
                .HasColumnName("lastlat");
            entity.Property(e => e.Lastlon)
                .HasColumnType("double")
                .HasColumnName("lastlon");
            entity.Property(e => e.Lasttime)
                .HasColumnType("long")
                .HasColumnName("lasttime");
            entity.Property(e => e.Ssid).HasColumnName("ssid");
            entity.Property(e => e.Type)
                .HasDefaultValue("W")
                .HasColumnName("type");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.ToTable("route");

            entity.Property(e => e.Id).HasColumnName("_id");
            entity.Property(e => e.Accuracy)
                .HasColumnType("float")
                .HasColumnName("accuracy");
            entity.Property(e => e.Altitude)
                .HasColumnType("double")
                .HasColumnName("altitude");
            entity.Property(e => e.BtVisible).HasColumnName("bt_visible");
            entity.Property(e => e.CellVisible).HasColumnName("cell_visible");
            entity.Property(e => e.Lat)
                .HasColumnType("double")
                .HasColumnName("lat");
            entity.Property(e => e.Lon)
                .HasColumnType("double")
                .HasColumnName("lon");
            entity.Property(e => e.RunId).HasColumnName("run_id");
            entity.Property(e => e.Time)
                .HasColumnType("long")
                .HasColumnName("time");
            entity.Property(e => e.WifiVisible).HasColumnName("wifi_visible");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
