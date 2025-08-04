using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CameraSearch.Repository;
using CameraSearch.Repository.Interfaces;
using CameraSearch.Services;
using CameraSearch.Services.Interfaces;
using everybody_codes.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CameraSearch.Cli {
    public class Program {
        public static async Task Main(string[] args) {
            try {
                using IHost host = CreateHostBuilder(args).Build();
                var app = host.Services.GetRequiredService<CameraDataApplication>();
                await app.RunAsync();
            } catch (Exception ex) {
                Console.WriteLine("A fatal error occurred. Application will exit.");
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureServices((context, services) => {
                    // Configure data settings from appsettings.json
                    services.Configure<DataSettings>(context.Configuration.GetSection("DataSettings"));

                    // Register services
                    services.AddSingleton<ICameraRepository, CameraRepository>();
                    services.AddSingleton<ICameraService, CameraService>();
                    services.AddSingleton<CameraDataApplication>();
                });
    }

    public class CameraDataApplication {
        private readonly ICameraService _cameraService;
        private readonly ILogger<CameraDataApplication> _logger;
        private List<Camera> _cameras;

        public CameraDataApplication(
            ICameraService cameraService,
            ILogger<CameraDataApplication> logger) {
            _cameraService = cameraService;
            _logger = logger;
        }

        public async Task InitializeAsync() {
            _cameras = await _cameraService.GetAllAsync();
        }

        public async Task RunAsync() {
            var proceed = "";
            do {
                try {
                    //var cameras = await _cameraService.GetAllAsync();
                    await InitializeAsync();
                    PrintCameras();
                    Console.WriteLine("Search camera by name");
                    var inputSearch = await Console.In.ReadLineAsync();
                    filterCameras(inputSearch);
                } catch (Exception ex) {
                    _logger.LogError(ex, "An error occurred while processing camera data");
                    throw;
                }
                Console.WriteLine("\nType 'no' to exit or any other key to continue...");
                proceed = Console.ReadLine()?.Trim().ToLower() ?? "";
            } while (proceed != "no");
        }

        private void PrintCameras() {
            try {
                _logger.LogInformation("Displaying all cameras:");
                Console.WriteLine("All cameras:");
                Console.WriteLine("ID   | Name  |Latitute   |Longitude  ");
                Console.WriteLine("-----|-------------------------------");
                foreach (var camera in _cameras) {
                    Console.WriteLine($"{camera.Number,-5} | {camera.Name,-30} | {camera.Latitude,8} | {camera.Longitude,8}");
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while processing camera data");
                throw;
            }
        }
        private void filterCameras(string inputSearch = "") {
            Console.WriteLine("ID   | Name  |Latitute   |Longitude  ");
            var filteredCameras = _cameras.Where(x => x.Name.Contains(inputSearch)).ToList();
            foreach (var camera in filteredCameras) {
                Console.WriteLine($"{camera.Number,-5} | {camera.Name,-30} | {camera.Latitude,8} | {camera.Longitude,8}");
            }
        }
    }
}