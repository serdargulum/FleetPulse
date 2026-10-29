"use client";

import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { forgotPassword } from "@/services/authService";
import {
    forgotPasswordSchema,
    ForgotPasswordFormData,
} from "@/schemas/authSchema";

export default function ForgotPasswordForm() {
    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting },
    } = useForm<ForgotPasswordFormData>({
        resolver: zodResolver(forgotPasswordSchema),
    });

    async function onSubmit(data: ForgotPasswordFormData) {
        await forgotPassword({
            email: data.email,
        });

        alert('Şifre sıfırlama e-postası adresinize gönderildi.');
    }

    return (
        <div className="w-full max-w-md">
            <div className="mb-8">
                <h1 className="text-2xl font-semibold text-gray-900">
                    Forgot password
                </h1>

                <p className="mt-2 text-sm text-gray-500">
                    Enter your email and we will send you a password reset link.
                </p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
                <div>
                    <label
                        htmlFor="email"
                        className="mb-2 block text-sm font-medium text-gray-700"
                    >
                        Email
                    </label>

                    <input
                        id="email"
                        type="email"
                        autoComplete="email"
                        {...register("email")}
                        className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm outline-none focus:border-gray-500 focus:ring-1 focus:ring-gray-500"
                        placeholder="you@example.com"
                    />

                    {errors.email && (
                        <p className="mt-1 text-sm text-red-600">
                            {errors.email.message}
                        </p>
                    )}
                </div>

                <button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full rounded-lg bg-gray-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-gray-800 disabled:cursor-not-allowed disabled:opacity-50"
                >
                    {isSubmitting ? "Sending..." : "Send reset link"}
                </button>
            </form>

            <p className="mt-6 text-center text-sm text-gray-500">
                Remember your password?{" "}
                <Link
                    href="/login"
                    className="font-medium text-gray-900 hover:underline"
                >
                    Sign in
                </Link>
            </p>
        </div>
    );
}