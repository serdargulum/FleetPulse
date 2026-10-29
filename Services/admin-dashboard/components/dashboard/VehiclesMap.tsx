"use client";

import { useEffect, useRef, useState } from "react";
import { setOptions, importLibrary } from "@googlemaps/js-api-loader";
import { TelemetryDto } from "@/types/TelemetryDto";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { faCar } from "@fortawesome/free-solid-svg-icons";

type VehicleLocation = TelemetryDto & { plate: string };

setOptions({ key: process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY! });

export default function VehiclesMap({ vehicles }: { vehicles: VehicleLocation[] }) {
    const mapRef = useRef<HTMLDivElement>(null);
    const map = useRef<google.maps.Map | null>(null);
    const markers = useRef<Map<string, google.maps.marker.AdvancedMarkerElement>>(new Map());
    const [mapReady, setMapReady] = useState(false);

    useEffect(() => {
        const initMap = async () => {
            if (!mapRef.current) return;

            const { Map } = await importLibrary("maps") as google.maps.MapsLibrary;
            await importLibrary("marker");

            map.current = new Map(mapRef.current, {
                center: { lat: 40.98, lng: 29.10 },
                zoom: 12,
                mapId: "DEMO_MAP_ID"
            });

            setMapReady(true);
        };

        initMap();
    }, []);

    useEffect(() => {
        if (!mapReady || !map.current || vehicles.length === 0) return;

        const updateMarkers = async () => {
            const { AdvancedMarkerElement } = await importLibrary("marker") as google.maps.MarkerLibrary;
            const bounds = new google.maps.LatLngBounds();

            vehicles.forEach(vehicle => {
                const position = { lat: vehicle.latitude, lng: vehicle.longitude };
                bounds.extend(position);

                let marker = markers.current.get(vehicle.plate);

                if (!marker) {
                    const content = document.createElement("div");
                    content.innerHTML = `<div class="flex flex-col items-center text-black">${icon(faCar).html.join("")}<span class="bg-white border border-gray-400 rounded px-2 py-1 text-xs font-bold shadow">${vehicle.plate}</span></div>`;

                    marker = new AdvancedMarkerElement({ map: map.current!, title: vehicle.plate, content });
                    markers.current.set(vehicle.plate, marker);
                    console.log("MARKER EKLENDİ:", vehicle.plate, position);
                }

                marker.position = position;
            });

            map.current!.fitBounds(bounds, 50);
        };

        updateMarkers();
    }, [vehicles, mapReady]);

    return <div ref={mapRef} className="h-[500px] w-full" />;
}