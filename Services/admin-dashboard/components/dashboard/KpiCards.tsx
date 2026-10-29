import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCar, faCircleCheck, faTriangleExclamation } from "@fortawesome/free-solid-svg-icons";

export default function KpiCards({ data }) {
    const stats = [
        { label: "Toplam Araç", value: data.totalVehicles, icon: faCar },
        { label: "Aktif Araç", value: data.activeVehicles, icon: faCircleCheck },
        { label: "Aktif Uyarı", value: data.activeAlerts, icon: faTriangleExclamation },
    ];

    return (
        <div className="grid grid-cols-12 gap-4">
            {stats.map((stat) => (
                <div key={stat.label} className="col-span-12 box md:col-span-4">
                    <div className="flex items-center gap-5">
                        <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-xl ">
                            <FontAwesomeIcon icon={stat.icon} className="text-3xl primary-color" />
                        </div>

                        <div>
                            <p className="text-sm text-gray-500">{stat.label}</p>
                            <p className="mt-1 text-3xl font-semibold">{stat.value}</p>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
}