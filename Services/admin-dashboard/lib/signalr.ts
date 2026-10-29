"use client";

import { getRealtimeToken } from "@/services/authService";
import { HubConnectionBuilder } from "@microsoft/signalr";

export const telemetryConnection = new HubConnectionBuilder()
    .withUrl(`${process.env.NEXT_PUBLIC_STREAM_URL}/hubs/telemetry`, {
        accessTokenFactory: async () => {
            return await getRealtimeToken();
        }
    })
    .withAutomaticReconnect()
    .build();

telemetryConnection.onclose((error) => {
    console.error("SignalR connection closed:", error);
});

telemetryConnection.onreconnecting((error) => {
    console.warn("SignalR reconnecting:", error);
});

telemetryConnection.onreconnected((connectionId) => {
    console.log("SignalR reconnected:", connectionId);
});