using CameraSearch.Repository.Interfaces;
using everybody_codes.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CameraSearch.Repository {
    public class CameraRepository : ICameraRepository {
        private readonly string _filePath;
        private readonly ILogger<CameraRepository> _logger;
        private const int InvalidCameraNumber = -1;
        public CameraRepository(IOptions<DataSettings> config, ILogger<CameraRepository> logger) {
            var appDataPath = Path.Combine(AppContext.BaseDirectory, config.Value.BaseDataPath);
            Directory.CreateDirectory(appDataPath);
            _filePath = Path.Combine(AppContext.BaseDirectory,
                            config.Value.BaseDataPath,
                            config.Value.FileName);
            _logger = logger;
        }
        public async Task<List<Camera>> GetAllAsync() {
            var cameras = new List<Camera>();

            if (!File.Exists(_filePath)) {
                _logger.LogError($"CSV file not found at path: {_filePath}");
                return cameras;
            }

            using var reader = new StreamReader(_filePath);
            await reader.ReadLineAsync(); // Skip header

            int lineNumber = 1;
            while (!reader.EndOfStream) {
                lineNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(';');
                if (values.Length < 3) {
                    _logger.LogWarning($"Line {lineNumber}: Insufficient data fields");
                    continue;
                }

                if (TryParseCamera(values, lineNumber, out var camera)) {
                    cameras.Add(camera);
                }
            }

            return cameras;
        }


        private bool TryParseCamera(string[] values, int lineNumber, out Camera camera) {
            camera = null;

            try {
                // Parse camera name and number
                var cameraName = values[0].Trim();
                var number = ExtractCameraNumber(cameraName);

                if (number == InvalidCameraNumber) {
                    _logger.LogWarning($"Line {lineNumber}: Invalid camera number format in '{cameraName}'");
                    return false;
                }

                // Parse location data
                if (!double.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var latitude)) {
                    _logger.LogWarning($"Line {lineNumber}: Invalid latitude format: {values[1]}");
                    return false;
                }

                if (!double.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var longitude)) {
                    _logger.LogWarning($"Line {lineNumber}: Invalid longitude format: {values[2]}");
                    return false;
                }

                var cleanName = cameraName;

                camera = new Camera(number, cleanName, latitude, longitude);
                return true;
            } catch (Exception ex) {
                _logger.LogWarning(ex, $"Line {lineNumber}: Unexpected error parsing camera data");
                return false;
            }
        }
        private int ExtractCameraNumber(string cameraName) {
            try {
                // Handle multiple formats:
                // 1. "UTR-CM-501 Neude rijbaan..."
                // 2. "501 Neude rijbaan..."
                // 3. "Camera 501: Neude rijbaan..."

                // Split by spaces and dashes
                var parts = cameraName.Split(new[] { ' ', '-', ':' }, StringSplitOptions.RemoveEmptyEntries);

                // Find the first part that's a number
                foreach (var part in parts) {
                    if (int.TryParse(part, out var number)) {
                        return number;
                    }
                }

                return InvalidCameraNumber;
            } catch {
                return InvalidCameraNumber;
            }
        }
    }
}

