import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGaugeHigh, faGasPump, faRoute, faRotate } from "@fortawesome/free-solid-svg-icons";
import { TelemetryDto } from "@/types/TelemetryDto";

export default function VehicleKpiCards({ data }: TelemetryDto) {

    const last = data[0] ?? null;

    if (!last) {
        console.error("VehicleKpiCards: Telemetry data is empty or missing.");
        return null;
    }

    return (
        <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">

            <div key="speed" className="box">
                <div className="flex items-center gap-5">
                    <div className="flex h-16 w-16 shrink-0 items-center justify-center">
                        <FontAwesomeIcon icon={faGaugeHigh} className="text-3xl primary-color" />
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">
                            Speed
                        </p>
                        <p className="mt-1 text-2xl font-semibold text-gray-900 dark:text-white">
                            {last.speed} km/h
                        </p>
                    </div>
                </div>
            </div>

            <div key="Fuel" className="box">
                <div className="flex items-center gap-5">
                    <div className="flex h-16 w-16 shrink-0 items-center justify-center">
                        <FontAwesomeIcon icon={faGasPump} className="text-3xl primary-color" />
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">
                            Fuel
                        </p>
                        <p className="mt-1 text-2xl font-semibold text-gray-900 dark:text-white">
                            {last.fuelLevel}%
                        </p>
                    </div>
                </div>
            </div>

            <div key="Engine Temperature" className="box">
                <div className="flex items-center gap-5">
                    <div className="flex h-16 w-16 shrink-0 items-center justify-center">
                        <FontAwesomeIcon icon={faRotate} className="text-3xl primary-color" />
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">
                            Engine Temperature
                        </p>
                        <p className="mt-1 text-2xl font-semibold text-gray-900 dark:text-white">
                            {last.engineTemperature}°C
                        </p>
                    </div>
                </div>
            </div>

        </div>
    );
}