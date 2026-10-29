import Link from "next/link";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCar } from "@fortawesome/free-solid-svg-icons";
import Plate from "../Plate";

function formatTime(lastUpdatedAt: string) {
    if (!lastUpdatedAt || lastUpdatedAt.startsWith("0001-01-01")) {
        return "-";
    }

    const diff = Date.now() - new Date(lastUpdatedAt).getTime();
    const minutes = Math.floor(diff / 60000);

    if (minutes < 1) return "Az önce";
    if (minutes < 60) return `${minutes} dk önce`;

    const hours = Math.floor(minutes / 60);

    if (hours < 24) return `${hours} saat önce`;

    const days = Math.floor(hours / 24);
    return `${days} gün önce`;
}

export default function VehicleTable({ data }) {
    return (
        <div className="box">
            <div className="border-b p-5">
                <h2 className="text-lg font-semibold uppercase">Araçlar</h2>
                <p className="text-sm text-gray-500">
                    Filo araçlarının anlık durumu
                </p>
            </div>

            <div className="overflow-x-auto">
                <table className="w-full text-left text-sm">
                    <thead className="border-b">
                        <tr>
                            <th className="px-5 py-3 font-medium">Plaka</th>
                            <th className="px-5 py-3 font-medium">Araç</th>
                            <th className="px-5 py-3 font-medium">Durum</th>
                            <th className="px-5 py-3 font-medium">Hız</th>
                            <th className="px-5 py-3 font-medium">Motor</th>
                            <th className="px-5 py-3 font-medium">Yakıt</th>
                            <th className="px-5 py-3 font-medium">Son Güncelleme</th>
                        </tr>
                    </thead>

                    <tbody className="divide-y text-gray-600">
                        {data.map((vehicle) => (
                            <tr key={vehicle.id}>

                                <td className="px-5 py-4">
                                    <Plate plate={vehicle.plate} vehicleId={vehicle.id} />
                                </td>

                                <td className="px-5 py-4 whitespace-nowrap">
                                    {vehicle.brand} {vehicle.model}
                                </td>

                                <td className="px-5 py-4">
                                    <span
                                        className={`rounded-full px-2.5 py-1 text-xs font-medium ${vehicle.isActive
                                            ? "bg-green-100 text-green-700"
                                            : "bg-gray-100 text-gray-600"
                                            }`}
                                    >
                                        {vehicle.isActive ? "Aktif" : "Pasif"}
                                    </span>
                                </td>

                                <td className="px-5 py-4">
                                    {vehicle.speed} km/h
                                </td>

                                <td className="px-5 py-4">
                                    {vehicle.engineTemperature > 0
                                        ? `${vehicle.engineTemperature} °C`
                                        : "-"}
                                </td>

                                <td className="px-5 py-4">
                                    {vehicle.fuelLevel}%
                                </td>

                                <td className="px-5 py-4 text-gray-500">
                                    {formatTime(vehicle.lastUpdatedAt)}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}