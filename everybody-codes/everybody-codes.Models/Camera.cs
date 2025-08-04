using System;

namespace everybody_codes.Models {
    public record Camera(
    int Number,
    string Name,
    double Latitude,
    double Longitude) {
        public string FullName => $"UTR-CM-{Number} {Name}";
        public string ShortName => Name.Split('-')[0].Trim();

        public bool IsValid =>
            Number > 0 &&
            !string.IsNullOrWhiteSpace(Name) &&
            Latitude >= -90 && Latitude <= 90 &&
            Longitude >= -180 && Longitude <= 180;
    }
}
