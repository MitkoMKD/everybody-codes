using everybody_codes.Models;
using System.Collections.Generic;

namespace CameraSearch.Web.ViewModels {
    public class CameraViewModel {
        public List<Camera> DivisibleBy3 { get; set; } = new List<Camera>();
        public List<Camera> DivisibleBy5 { get; set; } = new List<Camera>();
        public List<Camera> DivisibleBy3And5 { get; set; } = new List<Camera>();
        public List<Camera> NotDivisible { get; set; } = new List<Camera>();
        public List<Camera> AllCameras { get; set; } = new List<Camera>();
    }
}
