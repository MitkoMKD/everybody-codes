import { AfterViewInit, Component } from '@angular/core';
import * as L from 'leaflet';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID, Inject } from '@angular/core';
import { CameraViewModel } from '../../models/CameraViewModel';
import { CameraService } from './Service/camera.service';
import { Camera } from '../../models/Camera';

@Component({
  selector: 'app-map',
  imports: [CommonModule],
  templateUrl: './map.html',
  styleUrl: './map.scss'
})
export class Map implements AfterViewInit {
  private map: any;
  public vm: CameraViewModel | null = null;

  constructor(private cameraService: CameraService,
    @Inject(PLATFORM_ID) private platformId: Object) { }

  private initMap(cameras: Camera[]) {
    try {
      // Only proceed if we're in the browser and have cameras
      if (!isPlatformBrowser(this.platformId) || !cameras || cameras.length === 0) {
        console.warn('Map initialization skipped: not in browser or no cameras available');
        return;
      }

      // Find the first camera with valid coordinates for initial map center
      const validCamera = cameras.find(cam =>
        cam.latitude && cam.longitude && cam.latitude !== 0 && cam.longitude !== 0
      );

      // Set default coordinates (Netherlands) if no valid camera found
      const defaultLat = validCamera?.latitude || 52.0914;
      const defaultLng = validCamera?.longitude || 5.1115;

      // Initialize the map
      import('leaflet').then(L => {
        this.map = L.map('map').setView([defaultLat, defaultLng], 13);

        // Add the OpenStreetMap tiles
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
          attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(this.map);

        // Add camera markers
        let addedMarkers = 0;
        cameras.forEach(camera => {
          if (camera.latitude && camera.longitude && camera.latitude !== 0 && camera.longitude !== 0) {
            L.marker([camera.latitude, camera.longitude])
              .addTo(this.map)
              .bindPopup(`<b>${camera.name}</b><br>Camera #${camera.number}`);
            addedMarkers++;
          }
        });
        console.log(`Map initialized successfully with ${addedMarkers} cameras out of ${cameras.length} total cameras`);
      });
    } catch (error) {
      console.error('Error initializing map:', error);
    }
  }

  ngAfterViewInit(): void {
    this.cameraService.getViewModel().subscribe({
      next: (vm) => {
        this.vm = vm;
        if (isPlatformBrowser(this.platformId)) {
          this.initMap(vm.allCameras);
        }
      },
      error: (error) => {
        console.error('Error loading camera data:', error);
      }
    });
  }
}