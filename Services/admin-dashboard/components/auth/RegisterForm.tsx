"use client";

import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { register as registerUser } from "@/services/authService";
import {
    registerSchema,
    RegisterFormData,
} from "@/schemas/authSchema";

export default function RegisterForm() {

    const router = useRouter();

    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting },
    } = useForm<RegisterFormData>({
        resolver: zodResolver(registerSchema),
    });

    async function onSubmit(data: RegisterFormData) {

        await registerUser({
            name: data.name,
            email: data.email,
            password: data.password,
        });

        router.push("/login?registered=true");
    }

    return (
        <div className="w-full max-w-md">
            <div className="mb-8">
                <h1 className="text-2xl font-semibold text-gray-900">
                    Create account
                </h1>

                <p className="mt-2 text-sm text-gray-500">
                    Create your FleetPulse account.
                </p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
                <div>
                    <label
                        htmlFor="name"
                        className="mb-2 block text-sm font-medium text-gray-700"
                    >
                        Name
                    </label>

                    <input
                        id="name"
                        type="text"
                        autoComplete="name"
                        {...register("name")}
                        className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm outline-none focus:border-gray-500 focus:ring-1 focus:ring-gray-500"
                        placeholder="Your name"
                    />

                    {errors.name && (
                        <p className="mt-1 text-sm text-red-600">
                            {errors.name.message}
                        </p>
                    )}
                </div>

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

                <div>
                    <label
                        htmlFor="password"
                        className="mb-2 block text-sm font-medium text-gray-700"
                    >
                        Password
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
                    className="w-full rounded-lg bg-gray-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-gray-800 disabled:opacity-50"
                >
                    {isSubmitting ? "Creating..." : "Create account"}
                </button>
            </form>

            <p className="mt-6 text-center text-sm text-gray-500">
                Already have an account?{" "}
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
