const alertTypeStyles = {
    critical: "bg-red-100 text-red-700",
    warning: "bg-yellow-100 text-yellow-700",
};

export default function AlertType({ alertType }) {
    return (
        <span
            className={`rounded-full px-2.5 py-1 text-xs font-medium ${alertTypeStyles[
                alertType as keyof typeof alertTypeStyles
            ]
                }`}
        >
            {alertType === "critical"
                ? "Kritik"
                : "Uyarı"}
        </span>
    )
}