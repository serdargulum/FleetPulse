import api from "@/services/api";
import { Vehicle } from "@/types/Vehicle";

export async function getVehicle(id: string): Promise<Vehicle> {
    const response = await api.get<Vehicle>(`/api/vehicles/${id}`);
    return response.data;
}