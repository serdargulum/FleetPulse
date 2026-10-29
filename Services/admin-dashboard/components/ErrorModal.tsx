"use client";

import { useErrorStore } from "@/stores/errorStore";

export default function ErrorModal() {
    const message = useErrorStore((state) => state.message);
    const clearError = useErrorStore((state) => state.clearError);

    if (!message) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
            <div className="w-full max-w-md rounded-lg bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold">
                    Error
                </h2>

                <p className="mt-2 text-gray-600">
                    {message}
                </p>

                <button
                    type="button"
                    onClick={clearError}
                    className="mt-6 rounded bg-black px-4 py-2 text-white"
                >
                    OK
                </button>
            </div>
        </div>
    );
}