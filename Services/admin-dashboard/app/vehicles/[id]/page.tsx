"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import VehicleHeader from "@/components/vehicles/VehicleHeader";
import VehicleKpiCards from "@/components/vehicles/VehicleKpiCards";
import TelemetryChart from "@/components/vehicles/TelemetryChart";
import VehicleAlerts from "@/components/vehicles/VehicleAlerts";
import TelemetryTable from "@/components/vehicles/TelemetryTable";
import { getVehicle } from "@/services/vehicleService";
import { Vehicle } from "@/types/Vehicle";
import { telemetryConnection } from "@/lib/signalr";
import { TelemetryDto } from "@/types/TelemetryDto";
import VehicleMap from "@/components/vehicles/VehicleMap";

export default function VehiclePage() {
    const { id } = useParams<{ id: string }>();
    const [data, setData] = useState<Vehicle | null>(null);
    const [telemetries, setTelemetries] = useState<TelemetryDto[]>([]);
    const [isBroadcasting, setIsBroadcasting] = useState(false);

    useEffect(() => {
        if (!data) return;

        const handler = (telemetry: TelemetryDto) => {
            console.log(">>> TelemetryUpdated:", telemetry);
            setTelemetries(prev => [telemetry, ...prev].slice(0, 30));
        };

        const start = async () => {
            try {
                telemetryConnection.on("TelemetryUpdated", handler);

                if (telemetryConnection.state === "Disconnected") {
                    await telemetryConnection.start();
                    setIsBroadcasting(true);
                    console.log("SignalR connected:", telemetryConnection.state);
                }

                if (telemetryConnection.state !== "Connected") {
                    console.log("SignalR not ready:", telemetryConnection.state);
                    return;
                }

                console.log("Joining vehicle:", data.plate);

                await telemetryConnection.invoke("JoinVehicle", data.plate);

                console.log("Joined vehicle:", data.plate);
            } catch (error) {
                console.error("SignalR connection error:", error);
            }
        };

        start();

        return () => {

            telemetryConnection.off("TelemetryUpdated", handler);
            setIsBroadcasting(false);

            if (telemetryConnection.state === "Connected") {
                telemetryConnection.stop();
            }
        };
    }, [data]);

    useEffect(() => {
        if (!id) return;

        getVehicle(id)
            .then((result) => {
                setData(result);
                setTelemetries(result.latestTelemetry);
            })
            .catch((error) => console.log(">>> Vehicle error", error));

    }, [id]);

    if (!data) {
        return <div>Loading...</div>;
    }

    const slicedData = telemetries.slice(0, 30);

    return (
        <div className="space-y-6">
            <VehicleHeader data={data} />

            <VehicleKpiCards data={slicedData} />

            <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
                <TelemetryChart telemetries={slicedData} isBroadcasting={isBroadcasting} />

                <VehicleMap telemetries={telemetries} isBroadcasting={isBroadcasting} />
            </div>
            <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
                <VehicleAlerts alerts={data.alertList} isBroadcasting={isBroadcasting} />

                <TelemetryTable telemetries={slicedData} isBroadcasting={isBroadcasting} />
            </div>
        </div>
    );
}