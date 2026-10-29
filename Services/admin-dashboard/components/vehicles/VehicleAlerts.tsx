import formatTime from "@/lib/formatTime";
import AlertType from "../AlertType";
import BroadcastingIcon from "../BroadcastingIcon";

export default function VehicleAlerts({ alerts, isBroadcasting }: { isBroadcasting: boolean }) {

    return (
        <div className="box">
            <div className="flex items-center justify-between border-b border-gray-200 p-6">
                <h2 className="text-lg font-semibold">
                    Recent Alerts
                </h2>

                <BroadcastingIcon isBroadcasting={isBroadcasting} />
            </div>

            <div className="mt-4 divide-y divide-gray-100">

                {!alerts ?
                    <p className="py-8 text-center text-sm text-gray-400">
                        Aktif bir uyarı bulunmuyor.
                    </p>
                    :
                    alerts.map((alert, index) => (
                        <div
                            key={alert.vehicleId + "-" + index}
                            className="flex items-center justify-between gap-4 py-4"
                        >
                            <div>
                                <p className="text-sm text-gray-500">
                                    <b className="primary-color pr-3">{alert.alertType}</b>
                                    {alert.message}
                                </p>
                            </div>

                            <div className="flex items-center gap-3">
                                <AlertType alertType={alert.alertType} />

                                <span className="whitespace-nowrap text-sm text-gray-400">
                                    {formatTime(alert.createdAt)}
                                </span>
                            </div>
                        </div>
                    ))}
            </div>
        </div>
    );
}