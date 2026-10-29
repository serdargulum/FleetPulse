import api from "./api";

export interface RegisterRequest {
    name: string;
    email: string;
    password: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface ForgotPasswordRequest {
    email: string;
}

export interface ResetPasswordRequest {
    token: string;
    password: string;
}

export async function register(data: RegisterRequest) {
    const response = await api.post("/api/auth/register", data);

    return response.data;
}

export async function login(data: LoginRequest) {
    const response = await api.post("/api/auth/login", data);

    return response.data;
}

export async function forgotPassword(data: ForgotPasswordRequest) {
    const response = await api.post("/api/auth/forgot-password", data);

    return response.data;
}

export async function resetPassword(data: ResetPasswordRequest) {
    const response = await api.post("/api/auth/reset-password", data);

    return response.data;
}

export async function getRealtimeToken(): Promise<string> {
    // mevcut token alma mantığın
    return "";
}