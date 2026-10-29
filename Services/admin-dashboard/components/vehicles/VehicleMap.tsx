"use client";

import { useEffect, useRef, useState } from "react";
import { setOptions, importLibrary } from "@googlemaps/js-api-loader";
import { TelemetryDto } from "@/types/TelemetryDto";
import BroadcastingIcon from "../BroadcastingIcon";

setOptions({ key: process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY! });

export default function VehicleMap({ telemetries, isBroadcasting }: { telemetries: TelemetryDto[], isBroadcasting: boolean }) {
    const mapRef = useRef<HTMLDivElement>(null);
    const map = useRef<google.maps.Map | null>(null);
    const marker = useRef<google.maps.marker.AdvancedMarkerElement | null>(null);
    const polyline = useRef<google.maps.Polyline | null>(null);
    const [mapReady, setMapReady] = useState(false);

    useEffect(() => {
        const initMap = async () => {
            if (!mapRef.current) return;

            const { Map } = await importLibrary("maps") as google.maps.MapsLibrary;
            const { AdvancedMarkerElement } = await importLibrary("marker") as google.maps.MarkerLibrary;

            map.current = new Map(mapRef.current, {
                center: { lat: 40.98, lng: 29.10 },
                zoom: 14,
                mapId: "DEMO_MAP_ID"
            });

            marker.current = new AdvancedMarkerElement({ map: map.current });

            polyline.current = new google.maps.Polyline({
                map: map.current,
                path: [],
                geodesic: true,
                strokeWeight: 4
            });

            setMapReady(true);
        };

        initMap();
    }, []);

    useEffect(() => {
        if (!mapReady || !map.current || !marker.current || !polyline.current || telemetries.length === 0) return;

        const updateMap = async () => {
            const latest = telemetries[telemetries.length - 1];
            const latestPosition = { lat: latest.latitude, lng: latest.longitude };

            marker.current!.position = latestPosition;
            map.current!.panTo(latestPosition);

            const path = telemetries.map(item => `${item.latitude},${item.longitude}`).join("|");
            const key = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;
            if (!key) return;

            const response = await fetch(`https://roads.googleapis.com/v1/snapToRoads?path=${encodeURIComponent(path)}&interpolate=true&key=${key}`);

            if (!response.ok) {
                console.error("Roads API:", response.status);
                return;
            }

            const result = await response.json();
            const roadPath = result.snappedPoints?.map((point: any) => ({
                lat: point.location.latitude,
                lng: point.location.longitude
            })) ?? [];

            polyline.current!.setPath(roadPath);
        };

        updateMap();
    }, [telemetries, mapReady]);

    return (
        <div className="relative" >
            <div className="absolute right-3 top-3 z-10">
                <BroadcastingIcon isBroadcasting={isBroadcasting} />
            </div>
            <div ref={mapRef} className="h-[400px] w-full" />;
        </div>
    );
}