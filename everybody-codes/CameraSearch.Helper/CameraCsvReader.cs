using CsvHelper;
using everybody_codes.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CameraSearch.Helper {
    public class CameraCsvReader {
        public List<Camera> LoadCameras(string path) {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Camera>().ToList();
        }
    }
}
