"use client";

import { useEffect, useState } from "react";

import KpiCards from "@/components/dashboard/KpiCards";
import AlertsSummary from "@/components/dashboard/AlertsSummary";
import VehicleTable from "@/components/dashboard/VehicleTable";
import { getDashboard } from "@/services/dashboardService";
import VehiclesMap from "@/components/dashboard/VehiclesMap";

export default function DashboardPage() {
  const [data, setData] = useState(null);

  useEffect(() => {

    getDashboard()
      .then((result) => {
        setData(result);
      })
      .catch((error) => {
        console.log(">>> Dashboard error", error);
      });

    return () => { };
  }, []);

  if (!data) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">

      <KpiCards data={data.summary} />

      <VehiclesMap vehicles={data.positions} />

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <AlertsSummary data={data.alerts} />
        <VehicleTable data={data.vehicles} />
      </div>
    </div>
  );
}