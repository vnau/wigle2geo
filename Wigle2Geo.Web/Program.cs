using Wigle2Geo.Models;
using Wigle2Geo.Services;

namespace Wigle2Geo.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<HostOptions>(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            });

            builder.Services.AddSingleton(v =>
            {
                var context = new WiGleBackupContext("..\\backup.sqlite");
                return context;
            });

            builder.Services.AddSingleton<VendorResolverService>();
            builder.Services.AddSingleton<IVendorResolverService>(p => p.GetRequiredService<VendorResolverService>()!);
            builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<VendorResolverService>()!);
            builder.Services.AddHostedService<DatabaseService>();

            builder.Services.AddSingleton<IOuiSourceService, OuiSourceService>();

            builder.Services.AddSingleton<GeoJsonLocationsService>();
            builder.Services.AddSingleton<GeoJsonNetworksService>();

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("").AllowAnyHeader().WithMethods("GET");
                    });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
