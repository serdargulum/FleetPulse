import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGaugeHigh, faGasPump, faRoute, faRotate } from "@fortawesome/free-solid-svg-icons";

const kpis = [
    { label: "Speed", value: "82 km/h", icon: faGaugeHigh },
    { label: "Fuel", value: "68%", icon: faGasPump },
    { label: "Odometer", value: "124,820 km", icon: faRoute },
    { label: "Engine RPM", value: "2,450", icon: faRotate },
];

export default function VehicleKpiCards({ data }) {
    return (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {kpis.map((kpi) => (
                <div key={kpi.label} className="box">
                    <div className="flex items-center gap-5">
                        <div className="flex h-16 w-16 shrink-0 items-center justify-center">
                            <FontAwesomeIcon icon={kpi.icon} className="text-3xl primary-color" />
                        </div>

                        <div>
                            <p className="text-sm text-gray-500">{kpi.label}</p>
                            <p className="mt-1 text-2xl font-semibold text-gray-900 dark:text-white">{kpi.value}</p>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
}