import axios from "axios";
import { useErrorStore } from "@/stores/errorStore";

const api = axios.create({
    baseURL: process.env.NEXT_PUBLIC_API_URL,
    // required for cross-origin requests (local development)
    withCredentials: true,
    headers: {
        "Content-Type": "application/json",
    },
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        const status = error.response?.status;
        const requestUrl = error.config?.url ?? "";

        if (
            status === 401 &&
            !requestUrl.includes("/api/auth/")
        ) {
            console.log(status, requestUrl)
            if (typeof window !== "undefined") {
                window.location.href = "/login";
            }

            return Promise.reject(error);
        }

        const message = error.response?.data?.message ?? "An unexpected error occurred.";

        useErrorStore.getState().showError(message);

        return Promise.reject(error);
    }
);

export default api;