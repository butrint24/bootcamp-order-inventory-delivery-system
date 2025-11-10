"use client";

import { ProtectedRoute } from "@/components/protected-route";
import { Navbar } from "@/components/navbar";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useState, useEffect } from "react";
import { apiClient } from "@/lib/api-client";
import Link from "next/link";

interface Delivery {
  deliveryId: string;
  orderId: string;
  status: string;
  eta?: string | null;
}

export default function DeliveriesPage() {
  const [deliveries, setDeliveries] = useState<Delivery[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState("all");

  const fetchDeliveries = async () => {
    try {
      const response = await apiClient.get("/api/Delivery/my-deliveries");
      setDeliveries(response.data);
    } catch (error) {
      console.error("Failed to fetch deliveries:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDeliveries();
    const interval = setInterval(fetchDeliveries, 30000);
    return () => clearInterval(interval);
  }, []);

  const filteredDeliveries =
    statusFilter === "all"
      ? deliveries
      : deliveries.filter(
          (d) => d.status.toLowerCase() === statusFilter.toLowerCase()
        );

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-6xl mx-auto">
        <div className="mb-8 text-center">
          <h1 className="text-4xl font-bold text-blue-600">Track Deliveries</h1>
          <p className="text-gray-500">
            Monitor your order deliveries in real-time
          </p>
        </div>

        {/* Filter Section */}
        <Card className="mb-6 border border-blue-100 shadow-sm">
          <CardHeader>
            <CardTitle className="text-blue-700 text-lg">Filter Deliveries</CardTitle>
            <CardDescription>
              Select a status to filter your deliveries
            </CardDescription>
          </CardHeader>
          <CardContent>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="w-full md:w-1/3 px-4 py-2 border border-blue-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-blue-50 hover:bg-blue-100 transition"
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

        {/* Deliveries List */}
        {loading ? (
          <div className="text-center py-12 text-gray-600 animate-pulse">
            Loading deliveries...
          </div>
        ) : (
          <Card className="border border-blue-100 shadow-md">
            <CardHeader>
              <CardTitle className="text-blue-700">Your Deliveries</CardTitle>
              <CardDescription>
                Total: {filteredDeliveries.length} deliveries
              </CardDescription>
            </CardHeader>
            <CardContent>
              {filteredDeliveries.length === 0 ? (
                <div className="text-center py-10 text-gray-500">
                  No deliveries found.
                </div>
              ) : (
                <div className="grid gap-4 sm:grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
                  {filteredDeliveries.map((delivery) => (
                    <Link
                      key={delivery.deliveryId}
                      href={`/deliveries/${delivery.deliveryId}`}
                    >
                      <div className="p-5 border border-gray-200 rounded-xl hover:shadow-lg hover:-translate-y-1 transition-all bg-white cursor-pointer">
                        <div className="flex flex-col space-y-2">
                          <div>
                            <p className="text-sm text-gray-500 font-medium">
                              Order ID
                            </p>
                            <p className="font-semibold text-gray-800 truncate">
                              {delivery.orderId}
                            </p>
                          </div>

                          <div>
                            <p className="text-sm text-gray-500 font-medium">
                              Status
                            </p>
                            <span
                              className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${getStatusColor(
                                delivery.status
                              )}`}
                            >
                              {formatStatusText(delivery.status)}
                            </span>
                          </div>

                          <div>
                            <p className="text-sm text-gray-500 font-medium">
                              ETA
                            </p>
                            <p className="font-medium text-gray-800">
                              {delivery.eta
                                ? new Date(delivery.eta).toLocaleDateString()
                                : "No ETA available"}
                            </p>
                          </div>
                        </div>
                      </div>
                    </Link>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        )}
      </main>
    </ProtectedRoute>
  );
}

/* --- Helpers --- */
function getStatusColor(status: string): string {
  const colors: { [key: string]: string } = {
    PENDING: "bg-yellow-100 text-yellow-800",
    PROCESSING: "bg-blue-100 text-blue-800",
    ON_ROUTE: "bg-purple-100 text-purple-800",
    DELIVERED: "bg-green-100 text-green-800",
    CANCELED: "bg-red-100 text-red-800",
  };
  return colors[status] || "bg-gray-100 text-gray-800";
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
