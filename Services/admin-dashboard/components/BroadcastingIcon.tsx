
import { faCircleXmark, faTowerBroadcast } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export default function BroadcastingIcon({ isBroadcasting }: { isBroadcasting: boolean }) {

    return isBroadcasting ?
        (
            <span className="inline-flex h-7 w-7 items-center justify-center rounded-full bg-green-500 animate-pulse" >
                <FontAwesomeIcon icon={faTowerBroadcast} className="text-white" />
            </span>
        ) :
        (
            <span className="inline-flex h-7 w-7 items-center justify-center rounded-full bg-red-500" >
                <FontAwesomeIcon icon={faTowerBroadcast} className="text-white" />
            </span>
        );

}