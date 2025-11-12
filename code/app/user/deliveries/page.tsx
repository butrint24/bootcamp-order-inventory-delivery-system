"use client";

import { ProtectedRoute } from "@/components/protected-route";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { useState, useEffect } from "react";
import { apiClient } from "@/lib/api-client";

interface Delivery {
  deliveryId: string;
  orderId: string;
  status: string;
  eta?: string | null;
}

export default function UserDeliveriesPage() {
  const [deliveries, setDeliveries] = useState<Delivery[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState("all");

  useEffect(() => {
    const fetchDeliveries = async () => {
      try {
        const res = await apiClient.get("/api/Delivery/my-deliveries");
        setDeliveries(Array.isArray(res.data) ? res.data : []);
      } catch (err) {
        console.error("Failed to fetch deliveries:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchDeliveries();
  }, []);

  const filtered = statusFilter === "all"
    ? deliveries
    : deliveries.filter((d) => d.status.toLowerCase() === statusFilter.toLowerCase());

  return (
    <ProtectedRoute requiredRole="user">
      <main className="p-8 max-w-6xl mx-auto">
        <h1 className="text-3xl font-bold text-blue-700 mb-4">Your Deliveries</h1>
        <p className="text-gray-500 mb-6">Track the delivery progress of your orders</p>

        <Card className="mb-6">
          <CardHeader>
            <CardTitle className="text-blue-700">Filter by Status</CardTitle>
          </CardHeader>
          <CardContent>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="w-full md:w-1/3 px-3 py-2 border rounded-md focus:ring-2 focus:ring-blue-400"
            >
              <option value="all">All Deliveries</option>
              <option value="PENDING">Pending</option>
              <option value="PROCESSING">Processing</option>
              <option value="ON_ROUTE">On Route</option>
              <option value="DELIVERED">Delivered</option>
              <option value="CANCELED">Canceled</option>
            </select>
          </CardContent>
        </Card>

        {loading ? (
          <div className="text-center text-gray-500 py-10">Loading deliveries...</div>
        ) : filtered.length === 0 ? (
          <div className="text-center text-gray-500 py-10">No deliveries found.</div>
        ) : (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {filtered.map((d) => (
              <Card key={d.deliveryId} className="border border-blue-100 hover:shadow-md transition">
                <CardHeader>
                  <CardTitle className="text-blue-700">
                    Delivery #{d.deliveryId.slice(0, 8)}
                  </CardTitle>
                  <CardDescription>
                    Order ID: <span className="text-gray-700">{d.orderId}</span>
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <p className="text-gray-800 font-medium">
                    Status:{" "}
                    <span className={`px-2 py-1 rounded text-sm ${getStatusColor(d.status)}`}>
                      {formatStatusText(d.status)}
                    </span>
                  </p>
                  <p className="text-gray-600 mt-1">
                    ETA:{" "}
                    {d.eta ? new Date(d.eta).toLocaleDateString() : "Not available"}
                  </p>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </main>
    </ProtectedRoute>
  );
}

/* --- Helpers --- */
function getStatusColor(status: string): string {
  const colors: Record<string, string> = {
    PENDING: "bg-yellow-100 text-yellow-700",
    PROCESSING: "bg-blue-100 text-blue-700",
    ON_ROUTE: "bg-purple-100 text-purple-700",
    DELIVERED: "bg-green-100 text-green-700",
    CANCELED: "bg-red-100 text-red-700",
  };
  return colors[status] || "bg-gray-100 text-gray-700";
}

function formatStatusText(status: string): string {
  const map: Record<string, string> = {
    PENDING: "Pending",
    PROCESSING: "Processing",
    ON_ROUTE: "On Route",
    DELIVERED: "Delivered",
    CANCELED: "Canceled",
  };
  return map[status] || status;
}
