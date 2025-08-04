// Initialize map when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Initialize map centered on Utrecht
    const map = L.map('map').setView([52.0914, 5.1115], 13);

    // Add OpenStreetMap tiles
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);

    // Add markers for each camera
    cameraData.forEach(camera => {
        if (camera.latitude && camera.longitude) {
            L.marker([camera.latitude, camera.longitude])
                .addTo(map)
                .bindPopup(`<b>${camera.name}</b><br>Camera #${camera.number}`);
        }
    });
});