"use client";

import { TelemetryDto } from "@/types/TelemetryDto";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
} from "recharts";
import BroadcastingIcon from "../BroadcastingIcon";

export default function TelemetryChart({ telemetries, isBroadcasting }: { telemetries: TelemetryDto[], isBroadcasting: boolean }) {

    const chartData = [...telemetries].reverse().map(item => ({
        time: new Date(item.recordedAt).toLocaleTimeString("tr-TR", { hour: "2-digit", minute: "2-digit" }),
        speed: item.speed
    }));

    return (
        <div className="box">
            <div className="mb-6 flex items-center justify-between">
                <div>
                    <h2 className="text-lg font-semibold">Telemetry</h2>
                    <p className="text-sm text-gray-500">
                        Vehicle speed over the last 40 minutes
                    </p>
                </div>

                <BroadcastingIcon isBroadcasting={isBroadcasting} />

            </div>

            <div className="h-80">
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={chartData}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="time" tick={{ fontSize: 12 }} />
                        <YAxis unit="km/sa" tick={{ fontSize: 12 }} />
                        <Tooltip formatter={(value) => [`${value} km/sa`, "Hız"]} />
                        <Line type="monotone" dataKey="speed" strokeWidth={2} dot={false} />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
}