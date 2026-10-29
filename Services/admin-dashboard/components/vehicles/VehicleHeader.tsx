import Link from "next/link";
import { Vehicle } from "@/types/Vehicle";
import { faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface VehicleHeaderProps {
    data: Vehicle;
}

export default function VehicleHeader({ data }: VehicleHeaderProps) {
    return (
        <div className="flex items-center gap-5">
            <Link href="/dashboard" className="flex items-center gap-2 text-sm text-gray-500 hover:text-gray-900">
                <FontAwesomeIcon icon={faArrowLeft} />
                Dashboard
            </Link>

            <h1 className="text-2xl font-semibol bg-red-400 text-white pr-2 pl-2">
                {data.plate}
            </h1>

            <span className={`rounded-full px-2.5 py-1 text-xs font-medium ${data.isActive ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-600"}`}>
                {data.isActive ? "Active" : "Inactive"}
            </span>

            <p className="text-sm text-gray-500 font-bold">
                {data.brand} {data.model} · {data.year}
            </p>
        </div>
    );
}