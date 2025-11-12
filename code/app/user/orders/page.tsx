"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { ProtectedRoute } from "@/components/protected-route";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { apiClient } from "@/lib/api-client";

type Order = {
  orderId: string;
  orderNumber?: string;
  status: string;
  price: number;
  createdAt: string;
  items?: Array<{ productId: string; quantity: number }>;
};

export default function UserOrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const res = await apiClient.get("/api/Order/user-orders");
        setOrders(Array.isArray(res.data) ? res.data : []);
      } catch (err) {
        console.error("Failed to load user orders:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchOrders();
  }, []);

  return (
    <ProtectedRoute requiredRole="user">
      <main className="p-8 max-w-6xl mx-auto">
        <h1 className="text-3xl font-bold text-blue-700 mb-4">Your Orders</h1>
        <p className="text-gray-500 mb-6">Track your order history below</p>

        {loading ? (
          <div className="text-center text-gray-500 py-10">Loading orders...</div>
        ) : orders.length === 0 ? (
          <div className="text-center text-gray-500 py-10">
            You haven’t placed any orders yet.
          </div>
        ) : (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {orders.map((order) => (
              <Card
                key={order.orderId}
                className="hover:shadow-md transition-all border border-blue-100"
              >
                <CardHeader>
                  <CardTitle className="text-blue-700">
                    Order #{order.orderNumber ?? order.orderId.slice(0, 8)}
                  </CardTitle>
                  <CardDescription>
                    {new Date(order.createdAt).toLocaleDateString()}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <p className="font-semibold text-gray-800">
                    Status:{" "}
                    <span
                      className={`px-2 py-1 rounded text-sm ${getStatusColor(
                        order.status
                      )}`}
                    >
                      {order.status}
                    </span>
                  </p>
                  <p className="text-gray-600 mt-1">
                    {order.items?.length || 0} items — ${order.price.toFixed(2)}
                  </p>
                  <Link
                    href={`/user/orders/${order.orderId}`}
                    className="inline-block mt-3 text-blue-600 hover:underline font-medium"
                  >
                    View Details →
                  </Link>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </main>
    </ProtectedRoute>
  );
}

function getStatusColor(status: string): string {
  const colors: Record<string, string> = {
    PENDING: "bg-yellow-100 text-yellow-700",
    PROCESSING: "bg-blue-100 text-blue-700",
    ON_ROUTE: "bg-purple-100 text-purple-700",
    DELIVERED: "bg-green-100 text-green-700",
    CANCELED: "bg-red-100 text-red-700",
    OTHER: "bg-gray-100 text-gray-700",
  };
  return colors[status?.toUpperCase()] || "bg-gray-100 text-gray-700";
}
