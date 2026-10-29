"use client";

import { useEffect, useState } from "react";
import { faSun, faMoon } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Link from "next/link";

export default function Header() {
    const [dark, setDark] = useState(false);

    useEffect(() => {
        const saved = localStorage.getItem("theme");
        setDark(saved === "dark");
    }, []);

    useEffect(() => {
        document.documentElement.classList.toggle("dark", dark);
        localStorage.setItem("theme", dark ? "dark" : "light");
    }, [dark]);

    return (
        <header className="grid grid-cols-2 items-center px-6 py-4  mb-3">
            <div className="font-bold">
                <Link href="/dashboard" >
                    FLEETPULSE
                </Link>
            </div>
            <div className="flex justify-end">
                <button onClick={() => setDark(!dark)} className="rounded-lg border px-3 py-2 text-sm hover:bg-gray-100 dark:border-gray-700 dark:hover:bg-gray-800">
                    <FontAwesomeIcon icon={dark ? faSun : faMoon} />
                </button>
            </div>
        </header>
    );
}