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

  private async initMap(cameras: Camera[]) {
    try {
      const L = await import('leaflet');
      const map = L.map('map').setView([52.0914, 5.1115], 13);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
      }).addTo(map);

      cameras.forEach((camera) => {
        if (
          camera.latitude &&
          camera.longitude &&
          camera.latitude !== 0 &&
          camera.longitude !== 0
        ) {
          L.marker([camera.latitude, camera.longitude])
            .addTo(map)
            .bindPopup(`${camera.name}<br>Camera #${camera.number}`);
        }
      });

      console.log('Map initialized successfully with ' + cameras.length + ' cameras');
    } catch (error) {
      console.error('Error initializing map:', error);
    }
  }

  ngAfterViewInit(): void {
    this.cameraService.getViewModel().subscribe({
      next: async (vm) => {
        this.vm = vm;
        if (isPlatformBrowser(this.platformId)) {
          await this.initMap(vm.allCameras);
        }
      },
      error: (error) => {
        console.error('Error loading camera data:', error);
      }
    });
  }
}