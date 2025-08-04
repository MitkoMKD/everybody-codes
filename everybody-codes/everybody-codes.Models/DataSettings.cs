using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace everybody_codes.Models {
    public class DataSettings {
        public string BaseDataPath { get; set; }
        public string FileName { get; set; }
        public bool SkipInvalidLines { get; set; } = true;
        public bool LogSkippedLines { get; set; } = true;
    }
}
