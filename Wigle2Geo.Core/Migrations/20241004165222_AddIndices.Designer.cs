﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wigle2Geo.Models;

#nullable disable

namespace Wigle2Geo.Migrations
{
    [DbContext(typeof(WiGleBackupContext))]
    [Migration("20241004165222_AddIndices")]
    partial class AddIndices
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Wigle2Geo.Models.AndroidMetadatum", b =>
                {
                    b.Property<string>("Locale")
                        .HasColumnType("TEXT")
                        .HasColumnName("locale");

                    b.ToTable("android_metadata", (string)null);
                });

            modelBuilder.Entity("Wigle2Geo.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("_id");

                    b.Property<double>("Accuracy")
                        .HasColumnType("float")
                        .HasColumnName("accuracy");

                    b.Property<double>("Altitude")
                        .HasColumnType("double")
                        .HasColumnName("altitude");

                    b.Property<string>("Bssid")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("bssid");

                    b.Property<int>("External")
                        .HasColumnType("INTEGER")
                        .HasColumnName("external");

                    b.Property<double>("Lat")
                        .HasColumnType("double")
                        .HasColumnName("lat");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER")
                        .HasColumnName("level");

                    b.Property<double>("Lon")
                        .HasColumnType("double")
                        .HasColumnName("lon");

                    b.Property<long>("Time")
                        .HasColumnType("long")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.HasIndex("Bssid");

                    b.ToTable("location", (string)null);
                });

            modelBuilder.Entity("Wigle2Geo.Models.Network", b =>
                {
                    b.Property<string>("Bssid")
                        .HasColumnType("TEXT")
                        .HasColumnName("bssid");

                    b.Property<double>("Bestlat")
                        .HasColumnType("double")
                        .HasColumnName("bestlat");

                    b.Property<int>("Bestlevel")
                        .HasColumnType("INTEGER")
                        .HasColumnName("bestlevel");

                    b.Property<double>("Bestlon")
                        .HasColumnType("double")
                        .HasColumnName("bestlon");

                    b.Property<string>("Capabilities")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("capabilities");

                    b.Property<int>("Frequency")
                        .HasColumnType("INT")
                        .HasColumnName("frequency");

                    b.Property<double>("Lastlat")
                        .HasColumnType("double")
                        .HasColumnName("lastlat");

                    b.Property<double>("Lastlon")
                        .HasColumnType("double")
                        .HasColumnName("lastlon");

                    b.Property<long>("Lasttime")
                        .HasColumnType("long")
                        .HasColumnName("lasttime");

                    b.Property<string>("Ssid")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ssid");

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("W")
                        .HasColumnName("type");

                    b.HasKey("Bssid");

                    b.ToTable("network", (string)null);
                });

            modelBuilder.Entity("Wigle2Geo.Models.Route", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("_id");

                    b.Property<double>("Accuracy")
                        .HasColumnType("float")
                        .HasColumnName("accuracy");

                    b.Property<double>("Altitude")
                        .HasColumnType("double")
                        .HasColumnName("altitude");

                    b.Property<int>("BtVisible")
                        .HasColumnType("INTEGER")
                        .HasColumnName("bt_visible");

                    b.Property<int>("CellVisible")
                        .HasColumnType("INTEGER")
                        .HasColumnName("cell_visible");

                    b.Property<double>("Lat")
                        .HasColumnType("double")
                        .HasColumnName("lat");

                    b.Property<double>("Lon")
                        .HasColumnType("double")
                        .HasColumnName("lon");

                    b.Property<int>("RunId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("run_id");

                    b.Property<byte[]>("Time")
                        .IsRequired()
                        .HasColumnType("long")
                        .HasColumnName("time");

                    b.Property<int>("WifiVisible")
                        .HasColumnType("INTEGER")
                        .HasColumnName("wifi_visible");

                    b.HasKey("Id");

                    b.ToTable("route", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
