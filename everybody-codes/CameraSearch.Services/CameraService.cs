using CameraSearch.Repository.Interfaces;
using CameraSearch.Services.Interfaces;
using everybody_codes.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CameraSearch.Services {
    public class CameraService : ICameraService {

        private readonly ICameraRepository _cameraRepository;
        private readonly ILogger<CameraService> _logger;

        public CameraService(ICameraRepository cameraRepository, ILogger<CameraService> logger) {
            _logger = logger;
            _cameraRepository = cameraRepository;
        }
        public async Task<List<Camera>> GetAllAsync() {
            try {
                var cameras = await _cameraRepository.GetAllAsync();                

                return cameras.ToList();
            } catch (InvalidOperationException ex) {
                _logger.LogError(ex, "Invalid operation while retrieving cameras");
                throw new ApplicationException("Failed to retrieve cameras due to invalid operation", ex);
            } catch (ArgumentException ex) {
                _logger.LogError(ex, "Argument error while retrieving cameras");
                throw new ApplicationException("Failed to retrieve cameras due to invalid arguments", ex);
            } catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred while retrieving cameras");
                throw new ApplicationException("An unexpected error occurred while retrieving cameras", ex);
            }
        }

        public async Task<List<Camera>> GetFilteredCamerasAsync(string name = "", double? lat = null, double? lon = null) {
            try {
                var cameras = await _cameraRepository.GetAllAsync();

                var results = cameras.Where(camera =>
                    (!string.IsNullOrWhiteSpace(name) && camera.Name != null && camera.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) ||
                    (lat.HasValue && camera.Latitude == lat.Value) ||
                    (lon.HasValue && camera.Longitude == lon.Value)).ToList();

                return results;
            } catch (InvalidOperationException ex) {
                _logger.LogError(ex, "Invalid operation while retrieving cameras");
                throw new ApplicationException("Failed to retrieve cameras due to invalid operation", ex);
            } catch (ArgumentException ex) {
                _logger.LogError(ex, "Argument error while retrieving cameras");
                throw new ApplicationException("Failed to retrieve cameras due to invalid arguments", ex);
            } catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred while retrieving cameras");
                throw new ApplicationException("An unexpected error occurred while retrieving cameras", ex);
            }
        }
    }
}
