import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map, catchError, throwError, of } from 'rxjs';
import { Camera } from '../../../models/Camera';
import { CameraViewModel } from '../../../models/CameraViewModel';

@Injectable({ providedIn: 'root' })
export class CameraService {
    // Update this URL to match your actual API configuration
    private apiUrl = 'https://localhost:44334/api/Cameras/all'; // Use the correct port from your launchSettings.json

    constructor(private http: HttpClient) { }

    getAll(): Observable<Camera[]> {
        const headers = new HttpHeaders({ 'Accept': 'application/json' });
        return this.http.get<Camera[]>(this.apiUrl, { headers }).pipe(
            catchError(this.handleError)
        );
    }

    getViewModel(): Observable<CameraViewModel> {
        return this.getAll().pipe(
            map(cameras => {
                const viewModel: CameraViewModel = {
                    divisibleBy3: cameras.filter(c => c.number % 3 === 0 && c.number % 5 !== 0),
                    divisibleBy5: cameras.filter(c => c.number % 5 === 0 && c.number % 3 !== 0),
                    divisibleBy3And5: cameras.filter(c => c.number % 15 === 0),
                    notDivisible: cameras.filter(c => c.number % 3 !== 0 && c.number % 5 !== 0),
                    allCameras: cameras
                };
                return viewModel;
            }),
            catchError(error => {
                console.error('Failed to get cameras, using mock data:', error);
                // Fallback to mock data if API fails
                const mockCameras: Camera[] = [
                    { name: 'Camera 1', latitude: 41.9981, longitude: 21.4254, number: 1 },
                    { name: 'Camera 2', latitude: 41.9973, longitude: 21.4280, number: 2 },
                    { name: 'Camera 3', latitude: 42.0008, longitude: 21.4210, number: 3 },
                    { name: 'Camera 4', latitude: 41.9950, longitude: 21.4300, number: 4 },
                    { name: 'Camera 5', latitude: 42.0020, longitude: 21.4180, number: 5 },
                ];

                const mockViewModel: CameraViewModel = {
                    divisibleBy3: mockCameras.filter(c => c.number % 3 === 0 && c.number % 5 !== 0),
                    divisibleBy5: mockCameras.filter(c => c.number % 5 === 0 && c.number % 3 !== 0),
                    divisibleBy3And5: mockCameras.filter(c => c.number % 15 === 0),
                    notDivisible: mockCameras.filter(c => c.number % 3 !== 0 && c.number % 5 !== 0),
                    allCameras: mockCameras
                };
                return of(mockViewModel);
            })
        );
    }

    // Helper methods to determine camera status
    // Adjust these based on your actual Camera model properties
    private isActiveCamera(camera: Camera): boolean {
        // Example logic - adjust based on your Camera model
        // If you have a 'status' property:
        // return camera.status === 'active';

        // If you have an 'isOnline' boolean:
        // return camera.isOnline && !camera.isRecording;

        // Placeholder logic using number (replace with actual logic):
        return camera.number % 4 === 1;
    }

    private isOfflineCamera(camera: Camera): boolean {
        // Example logic - adjust based on your Camera model
        // return camera.status === 'offline' || !camera.isOnline;

        // Placeholder logic using number (replace with actual logic):
        return camera.number % 4 === 0;
    }

    private isRecordingCamera(camera: Camera): boolean {
        // Example logic - adjust based on your Camera model
        // return camera.isRecording === true;

        // Placeholder logic using number (replace with actual logic):
        return camera.number % 4 === 2;
    }

    private isIdleCamera(camera: Camera): boolean {
        // Example logic - adjust based on your Camera model
        // return camera.status === 'idle' || (camera.isOnline && !camera.isRecording);

        // Placeholder logic using number (replace with actual logic):
        return camera.number % 4 === 3;
    }

    private handleError(error: any) {
        console.error('API Error:', error);
        if (error.status === 0) {
            console.error('Network error - check if backend is running and CORS is configured');
        }
        return throwError(() => error);
    }
}