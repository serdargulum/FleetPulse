import { TelemetryDto } from "@/types/TelemetryDto";
import BroadcastingIcon from "../BroadcastingIcon";

export default function TelemetryTable({ telemetries, isBroadcasting }: { telemetries: TelemetryDto[], isBroadcasting: boolean }) {

    const tableData = telemetries.map(item => ({
        id: item.id,
        time: new Date(item.recordedAt).toLocaleTimeString("tr-TR", { hour: "2-digit", minute: "2-digit" }),
        speed: item.speed,
        fuel: item.fuelLevel,
        temperature: item.engineTemperature,
    }));

    return (
        <div className="box">
            <div className="flex items-center justify-between border-b border-gray-200 p-6">
                <h2 className="text-lg font-semibold">
                    Recent Telemetry
                </h2>

                <BroadcastingIcon isBroadcasting={isBroadcasting} />
            </div>

            <div className="overflow-x-auto">
                <table className="w-full text-left text-sm">
                    <thead className=" text-xs uppercase  ">
                        <tr>
                            <th className="px-6 py-3">Time</th>
                            <th className="px-6 py-3">Speed</th>
                            <th className="px-6 py-3">Fuel</th>
                            <th className="px-6 py-3">Temperature</th>
                        </tr>
                    </thead>

                    <tbody className="divide-y divide-gray-100">
                        {tableData.map((row) => (
                            <tr key={row.id}>
                                <td className="px-6 py-4">
                                    {String(row.time)}
                                </td>

                                <td className="px-6 py-4">
                                    {row.speed} km/h
                                </td>

                                <td className="px-6 py-4">
                                    {row.fuel}%
                                </td>

                                <td className="px-6 py-4">
                                    {row.temperature}°C
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}