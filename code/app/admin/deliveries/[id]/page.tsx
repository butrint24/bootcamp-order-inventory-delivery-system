"use client";

import { ProtectedRoute } from "@/components/protected-route";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { useEffect, useState } from "react";
import { apiClient } from "@/lib/api-client";
import { useParams, useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";

interface DeliveryDetail {
  deliveryId: string;
  orderId: string;
  status: string;
  eta?: string | null;
  createdAt: string;
  updatedAt: string;
  userId: string;
  isActive: boolean;
}

interface DeliveryUpdateDto {
  status?: string;
  eta?: string | null;
}

export default function DeliveryDetailPage() {
  const params = useParams();
  const router = useRouter();
  const { user, isAdmin } = useAuth();
  const deliveryId = params.id as string;

  const [delivery, setDelivery] = useState<DeliveryDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [updateData, setUpdateData] = useState<DeliveryUpdateDto>({ status: "", eta: "" });
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Fetch single delivery
  const fetchDelivery = async () => {
    try {
      const response = await apiClient.get(`/api/Delivery/${deliveryId}`);
      setDelivery(response.data);

      if (isAdmin && response.data) {
        setUpdateData({
          status: response.data.status,
          eta: response.data.eta || "",
        });
      }
    } catch (error) {
      console.error("Failed to fetch delivery:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDelivery();
    const interval = setInterval(fetchDelivery, 15000);
    return () => clearInterval(interval);
  }, [deliveryId]);

  // Handle admin update
  const handleUpdateDelivery = async () => {
    try {
      await apiClient.put(`/api/Delivery/${deliveryId}`, updateData);
      fetchDelivery();
      setIsModalOpen(false);
    } catch (error) {
      console.error("Failed to update delivery:", error);
    }
  };

  const formatStatus = (status: string) =>
    status.replace(/_/g, " ").toLowerCase().replace(/\b\w/g, (c) => c.toUpperCase());

  return (
    <ProtectedRoute requiredRole={isAdmin ? "admin" : "user"}>
      <main className="p-6 max-w-2xl mx-auto">
        <Button
          variant="ghost"
          className="mb-6"
          onClick={() => router.push("/admin/deliveries")}
        >
          ← Back to Deliveries
        </Button>

        {loading ? (
          <div className="text-center py-8 text-gray-600">Loading delivery details...</div>
        ) : delivery ? (
          <Card className="p-8 shadow-lg">
            <CardHeader className="mb-6">
              <CardTitle className="text-2xl font-bold text-blue-700">Delivery Details</CardTitle>
            </CardHeader>

            <CardContent className="space-y-6 text-lg">
              <InfoBlock label="Order ID" value={delivery.orderId} />
              <StatusBlock status={delivery.status} formatStatus={formatStatus} />
              <InfoBlock
                label="ETA"
                value={
                  delivery.eta
                    ? new Date(delivery.eta).toLocaleDateString()
                    : "No ETA available"
                }
              />
              <InfoBlock
                label="Created At"
                value={new Date(delivery.createdAt).toLocaleDateString()}
              />

              {isAdmin && (
                <Button className="w-full mt-4" onClick={() => setIsModalOpen(true)}>
                  Update Delivery
                </Button>
              )}

              {/* Update modal */}
              <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Update Delivery</DialogTitle>
                  </DialogHeader>
                  <div className="space-y-4 mt-2">
                    <div>
                      <label className="text-sm font-medium">Status</label>
                      <select
                        value={updateData.status}
                        onChange={(e) =>
                          setUpdateData({ ...updateData, status: e.target.value })
                        }
                        className="w-full px-3 py-2 border border-input rounded-md bg-background"
                      >
                        <option value="PENDING">Pending</option>
                        <option value="PROCESSING">Processing</option>
                        <option value="ON_ROUTE">On Route</option>
                        <option value="DELIVERED">Delivered</option>
                        <option value="CANCELED">Canceled</option>
                      </select>
                    </div>

                    <div>
                      <label className="text-sm font-medium">ETA</label>
                      <Input
                        type="date"
                        value={
                          updateData.eta
                            ? new Date(updateData.eta).toISOString().split("T")[0]
                            : ""
                        }
                        onChange={(e) =>
                          setUpdateData({ ...updateData, eta: e.target.value })
                        }
                      />
                    </div>

                    <Button className="w-full" onClick={handleUpdateDelivery}>
                      Save Changes
                    </Button>
                  </div>
                </DialogContent>
              </Dialog>
            </CardContent>
          </Card>
        ) : (
          <div className="text-center py-8 text-red-600">
            Failed to load delivery details.
          </div>
        )}
      </main>
    </ProtectedRoute>
  );
}

// Reusable info block
function InfoBlock({ label, value }: { label: string; value: string }) {
  return (
    <div className="p-4 bg-gray-50 rounded-lg">
      <p className="text-sm text-gray-500 mb-1">{label}</p>
      <p className="font-semibold">{value}</p>
    </div>
  );
}

// Status block with colors
function StatusBlock({
  status,
  formatStatus,
}: {
  status: string;
  formatStatus: (s: string) => string;
}) {
  const colors: { [key: string]: string } = {
    PENDING: "bg-gray-100 text-gray-800",
    PROCESSING: "bg-blue-100 text-blue-800",
    ON_ROUTE: "bg-purple-100 text-purple-800",
    DELIVERED: "bg-green-100 text-green-800",
    CANCELED: "bg-red-100 text-red-800",
  };
  return (
    <div className="p-4 bg-gray-50 rounded-lg">
      <p className="text-sm text-gray-500 mb-1">Status</p>
      <span
        className={`inline-block px-3 py-2 rounded text-base font-medium ${
          colors[status.toUpperCase()] || "bg-gray-100 text-gray-800"
        }`}
      >
        {formatStatus(status)}
      </span>
    </div>
  );
}
