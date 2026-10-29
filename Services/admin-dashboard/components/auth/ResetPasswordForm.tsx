"use client";

import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { resetPassword } from "@/services/authService";
import {
    resetPasswordSchema,
    ResetPasswordFormData,
} from "@/schemas/authSchema";

export default function ResetPasswordForm() {
    const searchParams = useSearchParams();
    const token = searchParams.get("token") ?? "";

    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting },
    } = useForm<ResetPasswordFormData>({
        resolver: zodResolver(resetPasswordSchema),
        defaultValues: {
            token,
        },
    });

    async function onSubmit(data: ResetPasswordFormData) {
        await resetPassword({
            token: data.token,
            password: data.password,
        });

        alert('Şifreniz değiştirildi.');
    }

    return (
        <div className="w-full max-w-md">
            <div className="mb-8">
                <h1 className="text-2xl font-semibold text-gray-900">
                    Reset password
                </h1>

                <p className="mt-2 text-sm text-gray-500">
                    Enter your new password below.
                </p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
                <input type="hidden" {...register("token")} />

                <div>
                    <label
                        htmlFor="password"
                        className="mb-2 block text-sm font-medium text-gray-700"
                    >
                        New password
                    </label>

                    <input
                        id="password"
                        type="password"
                        autoComplete="new-password"
                        {...register("password")}
                        className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm outline-none focus:border-gray-500 focus:ring-1 focus:ring-gray-500"
                        placeholder="••••••••"
                    />

                    {errors.password && (
                        <p className="mt-1 text-sm text-red-600">
                            {errors.password.message}
                        </p>
                    )}
                </div>

                <div>
                    <label
                        htmlFor="confirmPassword"
                        className="mb-2 block text-sm font-medium text-gray-700"
                    >
                        Confirm password
                    </label>

                    <input
                        id="confirmPassword"
                        type="password"
                        autoComplete="new-password"
                        {...register("confirmPassword")}
                        className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm outline-none focus:border-gray-500 focus:ring-1 focus:ring-gray-500"
                        placeholder="••••••••"
                    />

                    {errors.confirmPassword && (
                        <p className="mt-1 text-sm text-red-600">
                            {errors.confirmPassword.message}
                        </p>
                    )}
                </div>

                <button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full rounded-lg bg-gray-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-gray-800 disabled:cursor-not-allowed disabled:opacity-50"
                >
                    {isSubmitting ? "Resetting..." : "Reset password"}
                </button>
            </form>

            <p className="mt-6 text-center text-sm text-gray-500">
                <Link
                    href="/login"
                    className="font-medium text-gray-900 hover:underline"
                >
                    Back to sign in
                </Link>
            </p>
        </div>
    );
}