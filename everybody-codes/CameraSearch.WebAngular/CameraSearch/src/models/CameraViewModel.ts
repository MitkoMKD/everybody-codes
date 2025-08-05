import { Camera } from "./Camera";

export interface CameraViewModel {
    divisibleBy3: Camera[];
    divisibleBy5: Camera[];
    divisibleBy3And5: Camera[];
    notDivisible: Camera[];
    allCameras: Camera[];

    [key: string]: Camera[];
}