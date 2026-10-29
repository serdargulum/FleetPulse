import Link from "next/link";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCar } from "@fortawesome/free-solid-svg-icons";

export default function Plate({
    plate, vehicleId
}) {
    return (
        <Link href={`/vehicles/${vehicleId}`} className="license-plate">
            <FontAwesomeIcon icon={faCar} className="mr-2" />
            {plate}
        </Link>
    );
}