using CameraSearch.Services.Interfaces;
using CameraSearch.Web.ViewModels;
using everybody_codes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CameraSearch.Web.Controllers {
    public class CamerasController : Controller {
        private readonly ICameraService _cameraService;
        private readonly ILogger<CamerasController> _logger;

        public CamerasController(ICameraService cameraService, ILogger<CamerasController> logger) {
            _cameraService = cameraService;
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync() {
            try {
                ViewData["Title"] = "Security cameras Utrecht";

                var cameras = await _cameraService.GetAllAsync();

                if (cameras == null) {
                    _logger.LogWarning("Camera service returned null");
                    return View(new CameraViewModel {
                        DivisibleBy3 = new List<Camera>(),
                        DivisibleBy5 = new List<Camera>(),
                        DivisibleBy3And5 = new List<Camera>(),
                        NotDivisible = new List<Camera>(),
                        AllCameras = new List<Camera>()
                    });
                }

                var viewModel = new CameraViewModel {
                    DivisibleBy3 = cameras.Where(c => c.Number % 3 == 0 && c.Number % 5 != 0).ToList(),
                    DivisibleBy5 = cameras.Where(c => c.Number % 5 == 0 && c.Number % 3 != 0).ToList(),
                    DivisibleBy3And5 = cameras.Where(c => c.Number % 15 == 0).ToList(),
                    NotDivisible = cameras.Where(c => c.Number % 3 != 0 && c.Number % 5 != 0).ToList(),
                    AllCameras = cameras
                };

                return View(viewModel);
            } catch (ApplicationException ex) {
                _logger.LogError(ex, "Application error occurred while loading camera index");
                ViewBag.ErrorMessage = "Unable to load cameras at this time. Please try again later.";
                return View("Error");
            } catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred in camera index");
                ViewBag.ErrorMessage = "An unexpected error occurred. Please contact support if the problem persists.";
                return View("Error");
            }
        }
    }
}
