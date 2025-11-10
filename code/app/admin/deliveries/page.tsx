"use client";

import { ProtectedRoute } from "@/components/protected-route";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useState, useEffect } from "react";
import { apiClient } from "@/lib/api-client";
import Link from "next/link";

interface Delivery {
  deliveryId: string;
  orderId: string;
  status: string;
  eta?: string | null;
  createdAt: string;
  updatedAt: string;
  userId: string;
  isActive: boolean;
}

export default function AdminDeliveriesPage() {
  const [deliveries, setDeliveries] = useState<Delivery[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState("all");

  const fetchDeliveries = async () => {
    try {
      const response = await apiClient.get("/api/Delivery");
      setDeliveries(response.data);
    } catch (error) {
      console.error("Failed to fetch deliveries:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDeliveries();
  }, []);

  const filteredDeliveries =
    statusFilter === "all" ? deliveries : deliveries.filter((d) => d.status === statusFilter);

  const formatStatus = (status: string) =>
    status.replace(/_/g, " ").toLowerCase().replace(/\b\w/g, (c) => c.toUpperCase());

  return (
    <ProtectedRoute requiredRole="admin">
      <main className="p-6 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-blue-700">Delivery Management</h1>
          <p className="text-gray-600">Manage and track all deliveries</p>
        </div>

        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Filter</CardTitle>
          </CardHeader>
          <CardContent>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="px-3 py-2 border border-input rounded-md bg-background"
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
          <div className="text-center py-8 text-gray-600">Loading deliveries...</div>
        ) : (
          <Card>
            <CardHeader>
              <CardTitle>All Deliveries</CardTitle>
              <CardDescription>Total: {filteredDeliveries.length} deliveries</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b">
                      <th className="text-left py-2 px-4 font-medium">Order Id</th>
                      <th className="text-left py-2 px-4 font-medium">Status</th>
                      <th className="text-left py-2 px-4 font-medium">ETA</th>
                      <th className="text-left py-2 px-4 font-medium">Created At</th>
                      <th className="text-left py-2 px-4 font-medium">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {filteredDeliveries.map((delivery) => (
                      <tr key={delivery.deliveryId} className="border-b hover:bg-gray-50">
                        <td className="py-2 px-4 font-medium">{delivery.orderId}</td>
                        <td className="py-2 px-4">
                          <span
                            className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(
                              delivery.status
                            )}`}
                          >
                            {formatStatus(delivery.status)}
                          </span>
                        </td>
                        <td className="py-2 px-4 text-sm">
                          {delivery.eta ? new Date(delivery.eta).toLocaleDateString() : "-"}
                        </td>
                        <td className="py-2 px-4 text-sm text-gray-500">
                          {new Date(delivery.createdAt).toLocaleDateString()}
                        </td>
                        <td className="py-2 px-4">
                          <Link href={`/admin/deliveries/${delivery.deliveryId}`}>
                            <Button size="sm" variant="outline">
                              View
                            </Button>
                          </Link>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>

                {filteredDeliveries.length === 0 && (
                  <div className="text-center py-8 text-gray-500">No deliveries found</div>
                )}
              </div>
            </CardContent>
          </Card>
        )}
      </main>
    </ProtectedRoute>
  );
}

function getStatusColor(status: string): string {
  const colors: { [key: string]: string } = {
    PENDING: "bg-gray-100 text-gray-800",
    PROCESSING: "bg-blue-100 text-blue-800",
    ON_ROUTE: "bg-purple-100 text-purple-800",
    DELIVERED: "bg-green-100 text-green-800",
    CANCELED: "bg-red-100 text-red-800",
  };
  return colors[status.toUpperCase()] || "bg-gray-100 text-gray-800";
}
