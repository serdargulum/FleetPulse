export interface TelemetryDto {
    id: string;
    vehicleId: string;
    latitude: number;
    longitude: number;
    speed: number;
    engineTemperature: number;
    fuelLevel: number;
    recordedAt: string;
}