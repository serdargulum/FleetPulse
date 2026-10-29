import formatTime from "@/lib/formatTime";
import AlertType from "../AlertType";
import Plate from "../Plate";

export default function AlertsSummary({ data }) {
    return (
        <div className="box">
            <div className="mb-4">
                <h2 className="text-lg font-semibold">Son Uyarılar</h2>
                <p className="text-sm text-gray-500">
                    Araçlardan gelen son uyarılar
                </p>
            </div>

            <div className="divide-y">
                {data.length === 0 ? (
                    <p className="py-8 text-center text-sm text-gray-400">
                        Aktif bir uyarı bulunmuyor.
                    </p>
                ) : (
                    data.map((alert, index) => (
                        <div
                            key={alert.vehicleId + "-" + index}
                            className="flex items-center justify-between gap-4 py-4"
                        >
                            <div>
                                <Plate plate={alert.plate} vehicleId={alert.vehicleId} />

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
                    ))
                )}
            </div>
        </div>
    );
}