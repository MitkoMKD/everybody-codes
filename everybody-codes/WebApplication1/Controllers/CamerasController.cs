using CameraSearch.Services.Interfaces;
using everybody_codes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CameraSearch.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CamerasController : ControllerBase {
        private readonly ICameraService _cameraService;
        private readonly ILogger<CamerasController> _logger;


        public CamerasController(ICameraService cameraService, ILogger<CamerasController> logger)
        {
            _cameraService = cameraService;
            _logger = logger;
        }

        [HttpGet("byname")]
        public async Task<IActionResult> GetAsync([FromQuery] string name = "") {
            try {
                if (name == null) {
                    name = string.Empty;
                }

                var allCameras = await _cameraService.GetAllAsync();

                if (allCameras == null) {
                    _logger.LogWarning("Camera service returned null for Get method with name: {Name}", name);
                    return Ok(new List<Camera>());
                }

                var results = allCameras.Where(x => x.Name != null && x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

                return Ok(results);
            } catch (ApplicationException ex) {
                _logger.LogError(ex, "Application error occurred while searching cameras by name: {Name}", name);
                return StatusCode(500, new { error = "Unable to retrieve cameras at this time", details = ex.Message });
            } catch (ArgumentException ex) {
                _logger.LogError(ex, "Invalid argument while searching cameras by name: {Name}", name);
                return BadRequest(new { error = "Invalid search parameters", details = ex.Message });
            } catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred while searching cameras by name: {Name}", name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = "Please contact support if the problem persists" });
            }
        }

        //GET /api/cameras? name = central
        //GET /api/cameras? lat = 52.1
        //GET /api/cameras? lon = 5.12
        //GET /api/cameras? name = market & lat = 52.1
        [HttpGet("search")]
        public async Task<IActionResult> GetFilteredCamerasAsync([FromQuery] string name = "", [FromQuery] double? lat = null, [FromQuery] double? lon = null) {
            try {
                if (name == null) {
                    name = string.Empty;
                }

                var getFilteredCameras = await _cameraService.GetFilteredCamerasAsync(name,lat,lon);

                if (getFilteredCameras == null) {
                    _logger.LogWarning("Camera service returned null for Get method with name: {Name}", name);
                    return Ok(new List<Camera>());
                }

                return Ok(getFilteredCameras);
            } catch (ApplicationException ex) {
                _logger.LogError(ex, "Application error occurred while searching cameras by name: {Name}", name);
                return StatusCode(500, new { error = "Unable to retrieve cameras at this time", details = ex.Message });
            } catch (ArgumentException ex) {
                _logger.LogError(ex, "Invalid argument while searching cameras by name: {Name}", name);
                return BadRequest(new { error = "Invalid search parameters", details = ex.Message });
            } catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred while searching cameras by name: {Name}", name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = "Please contact support if the problem persists" });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync() {
            try {
                var results = await _cameraService.GetAllAsync();

                if (results == null) {
                    _logger.LogWarning("Camera service returned null for GetAll method");
                    return Ok(new List<Camera>());
                }

                return Ok(results);
            } catch (ApplicationException ex) {
                _logger.LogError(ex, "Application error occurred while retrieving all cameras");
                return StatusCode(500, new { error = "Unable to retrieve cameras at this time", details = ex.Message });
            } catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all cameras");
                return StatusCode(500, new { error = "An unexpected error occurred", details = "Please contact support if the problem persists" });
            }
        }
    }
}
