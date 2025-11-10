"use client";

import { ProtectedRoute } from "@/components/protected-route";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { useEffect, useState } from "react";
import { apiClient } from "@/lib/api-client";
import { useAuth } from "@/lib/auth-context";

type UiOrder = {
  id: string;
  orderNumber: string;
  customerEmail: string;
  status: string;
  totalAmount: number;
  createdAt: string;
};

export default function AdminOrdersPage() {
  const { user } = useAuth();
  const [orders, setOrders] = useState<UiOrder[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedOrder, setSelectedOrder] = useState<UiOrder | null>(null);
  const [statusFilter, setStatusFilter] = useState("all");

  // Normalize data for consistent UI
  const normalize = (raw: any): UiOrder => {
    const id = String(raw.orderId ?? raw.id ?? "");
    const email = String(raw.email ?? raw.userEmail ?? raw.customerEmail ?? "-");
    const status = String(raw.status ?? "OTHER");
    const createdAt = String(raw.createdAt ?? new Date().toISOString());
    const price =
      Number(raw.price ?? raw.totalAmount ?? 0) ||
      (Array.isArray(raw.items)
        ? raw.items.reduce(
            (sum: number, it: any) =>
              sum + Number(it.quantity ?? 0) * Number(it.price ?? 0),
            0
          )
        : 0);

    return {
      id,
      orderNumber: String(raw.orderNumber ?? id.slice(0, 8).toUpperCase()),
      customerEmail: email,
      status,
      totalAmount: Number(price),
      createdAt,
    };
  };

  const fetchOrders = async () => {
    try {
      setLoading(true);
      const res = await apiClient.get("/api/Order", { params: { pageNumber: 1, pageSize: 200 } });
      const data = res.data?.items ?? res.data ?? [];
      setOrders(data.map((o: any) => normalize(o)));
    } catch (err) {
      console.error("Failed to fetch orders:", err);
      setOrders([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const updateOrderStatus = async (orderId: string, newStatus: string) => {
    try {
      const { data: raw } = await apiClient.get(`/api/Order/${orderId}`);

      const payload = {
        ...raw,
        Status: String(newStatus).toUpperCase(),
      };

      await apiClient.put(`/api/Order/${orderId}`, payload);
      setSelectedOrder(null);
      fetchOrders();
    } catch (e: any) {
      const errorMessage =
        e?.response?.data?.message ||
        e?.message ||
        "Failed to update order.";
      console.error("PUT /api/Order/{id} failed:", e);
      alert(errorMessage);
    }
  };

  const filtered =
    statusFilter === "all"
      ? orders
      : orders.filter((o) => o.status.toLowerCase() === statusFilter);

  return (
    <ProtectedRoute requiredRole="admin">
      <main className="p-8 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-blue-700">Orders Management</h1>
          <p className="text-gray-500">View and manage all customer orders</p>
        </div>

        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Filter</CardTitle>
          </CardHeader>
          <CardContent>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-md bg-white"
            >
              <option value="all">All</option>
              <option value="pending">Pending</option>
              <option value="confirmed">Confirmed</option>
              <option value="shipped">Shipped</option>
              <option value="delivered">Delivered</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </CardContent>
        </Card>

        {loading ? (
          <div className="text-center py-8 text-gray-500">Loading orders...</div>
        ) : (
          <Card>
            <CardHeader>
              <CardTitle>All Orders</CardTitle>
              <CardDescription>Total: {filtered.length} orders</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead>
                    <tr className="border-b bg-gray-50">
                      <th className="text-left py-2 px-4 font-medium">Order #</th>
                      <th className="text-left py-2 px-4 font-medium">Customer</th>
                      <th className="text-left py-2 px-4 font-medium">Status</th>
                      <th className="text-left py-2 px-4 font-medium">Amount</th>
                      <th className="text-left py-2 px-4 font-medium">Date</th>
                      <th className="text-left py-2 px-4 font-medium">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {filtered.map((o) => (
                      <tr key={o.id} className="border-b hover:bg-gray-50 transition">
                        <td className="py-2 px-4 font-medium">{o.orderNumber}</td>
                        <td className="py-2 px-4">{o.customerEmail}</td>
                        <td className="py-2 px-4">
                          <span
                            className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(
                              o.status
                            )}`}
                          >
                            {o.status}
                          </span>
                        </td>
                        <td className="py-2 px-4">${o.totalAmount.toFixed(2)}</td>
                        <td className="py-2 px-4 text-gray-500">
                          {new Date(o.createdAt).toLocaleDateString()}
                        </td>
                        <td className="py-2 px-4">
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={() => setSelectedOrder(o)}
                          >
                            View
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>

                {filtered.length === 0 && (
                  <div className="text-center py-8 text-gray-500">
                    No orders found
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        )}

        {selectedOrder && (
          <Dialog open={!!selectedOrder} onOpenChange={() => setSelectedOrder(null)}>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Order Details</DialogTitle>
                <DialogDescription>
                  Order #{selectedOrder.orderNumber}
                </DialogDescription>
              </DialogHeader>

              <div className="space-y-4">
                <p><strong>Customer:</strong> {selectedOrder.customerEmail}</p>
                <p><strong>Status:</strong> {selectedOrder.status}</p>
                <p><strong>Total:</strong> ${selectedOrder.totalAmount.toFixed(2)}</p>

                <label className="text-sm font-medium">Update Status</label>
                <select
                  defaultValue={selectedOrder.status.toLowerCase()}
                  onChange={(e) => updateOrderStatus(selectedOrder.id, e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md bg-white"
                >
                  <option value="pending">Pending</option>
                  <option value="confirmed">Confirmed</option>
                  <option value="shipped">Shipped</option>
                  <option value="delivered">Delivered</option>
                  <option value="cancelled">Cancelled</option>
                </select>
              </div>
            </DialogContent>
          </Dialog>
        )}
      </main>
    </ProtectedRoute>
  );
}

function getStatusColor(status: string): string {
  const s = status.toLowerCase();
  const colors: Record<string, string> = {
    pending: "bg-yellow-100 text-yellow-800",
    confirmed: "bg-blue-100 text-blue-800",
    shipped: "bg-purple-100 text-purple-800",
    delivered: "bg-green-100 text-green-800",
    cancelled: "bg-red-100 text-red-800",
  };
  return colors[s] || "bg-gray-100 text-gray-800";
}
