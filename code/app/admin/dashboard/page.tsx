"use client";

import { ProtectedRoute } from "@/components/protected-route";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useEffect, useState } from "react";
import { apiClient } from "@/lib/api-client";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from "recharts";

type Stat = {
  totalOrders: number;
  pendingDeliveries: number;
  lowStockItems: number;
  totalInventoryValue: number;
  ordersPerDay: { day: string; count: number }[];
  statusDistribution: { name: string; value: number }[];
};

const COLORS = ["#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6"];

export default function AdminDashboardPage() {
  const [stats, setStats] = useState<Stat | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const results = await Promise.allSettled([
          apiClient.get("/api/Order", { params: { pageNumber: 1, pageSize: 200 } }),
          apiClient.get("/api/Delivery", { params: { pageNumber: 1, pageSize: 200 } }),
          apiClient.get("/api/Product"),
        ]);

        const pick = (i: number) => {
          const r = results[i];
          if (r.status === "fulfilled") return r.value.data?.items ?? r.value.data ?? [];
          console.warn(["Order", "Delivery", "Product"][i], "failed:", r);
          return [];
        };

        const orders = pick(0);
        const deliveries = pick(1);
        const products = pick(2);

        const totalOrders = orders.length;
        const pendingDeliveries = deliveries.filter((d: any) => {
          const s = String(d.status ?? d.Status ?? "").toLowerCase();
          return s && s !== "delivered";
        }).length;
        const lowStockItems = products.filter((p: any) => {
          const qty = Number(p.quantity ?? p.stock ?? 0);
          const reorder = Number(p.reorderLevel ?? p.minStock ?? 0);
          return reorder > 0 && qty < reorder;
        }).length;
        const totalInventoryValue = products.reduce((sum: number, p: any) => {
          const price = Number(p.price ?? 0);
          const qty = Number(p.stock ?? 0);
          return sum + price * qty;
        }, 0);

        const dayOf = (dt: any) => (dt ? String(dt).split("T")[0] : "");
        const countsPerDay = new Map<string, number>();
        orders.forEach((o: any) => {
          const day = dayOf(o.createdAt ?? o.CreatedAt);
          if (!day) return;
          countsPerDay.set(day, (countsPerDay.get(day) ?? 0) + 1);
        });
        const ordersPerDay = Array.from(countsPerDay.entries())
          .sort((a, b) => a[0].localeCompare(b[0]))
          .slice(-7)
          .map(([day, count]) => ({ day, count }));

        const statusMap = new Map<string, number>();
        orders.forEach((o: any) => {
          const s = String(o.status ?? o.Status ?? "OTHER").toUpperCase();
          statusMap.set(s, (statusMap.get(s) ?? 0) + 1);
        });
        const statusDistribution = Array.from(statusMap.entries()).map(([name, value]) => ({
          name,
          value,
        }));

        setStats({
          totalOrders,
          pendingDeliveries,
          lowStockItems,
          totalInventoryValue: Number(totalInventoryValue.toFixed(2)),
          ordersPerDay,
          statusDistribution,
        });
      } catch (err) {
        console.error("Admin stats error:", err);
        setStats(null);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  return (
    <ProtectedRoute requiredRole="admin">
      <main className="p-8 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-blue-700">Admin Dashboard</h1>
          <p className="text-gray-500">Monitor orders, deliveries, and stock performance</p>
        </div>

        {loading ? (
          <div className="text-center py-8 text-gray-500">Loading dashboard...</div>
        ) : !stats ? (
          <div className="text-center py-8 text-red-600">Failed to load dashboard</div>
        ) : (
          <>
            {/* Stat Cards */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
              <Card className="transition-all duration-300 hover:shadow-md hover:scale-[1.02]">
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-gray-500">Total Orders</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold text-blue-700">{stats.totalOrders}</div>
                </CardContent>
              </Card>

              <Card className="transition-all duration-300 hover:shadow-md hover:scale-[1.02]">
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-gray-500">Pending Deliveries</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold text-amber-600">{stats.pendingDeliveries}</div>
                </CardContent>
              </Card>

              <Card className="transition-all duration-300 hover:shadow-md hover:scale-[1.02]">
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-gray-500">Low Stock Items</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold text-red-600">{stats.lowStockItems}</div>
                </CardContent>
              </Card>

              <Card className="transition-all duration-300 hover:shadow-md hover:scale-[1.02]">
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-gray-500">Inventory Value</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold text-green-600">
                    ${stats.totalInventoryValue.toLocaleString()}
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Charts */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <Card className="transition-all duration-300 hover:shadow-md hover:scale-[1.01]">
                <CardHeader>
                  <CardTitle className="text-blue-700">Orders Per Day</CardTitle>
                  <CardDescription>Last 7 days</CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={stats.ordersPerDay}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="day" />
                      <YAxis allowDecimals={false} />
                      <Tooltip />
                      <Bar dataKey="count" fill="#3b82f6" radius={[4, 4, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>

              <Card className="transition-all duration-300 hover:shadow-md hover:scale-[1.01]">
                <CardHeader>
                  <CardTitle className="text-blue-700">Order Status Distribution</CardTitle>
                  <CardDescription>Current order status breakdown</CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <PieChart>
                      <Pie
                        data={stats.statusDistribution}
                        cx="50%"
                        cy="50%"
                        dataKey="value"
                        outerRadius={90}
                        label={({ name, value }) => `${name}: ${value}`}
                      >
                        {stats.statusDistribution.map((_, i) => (
                          <Cell key={i} fill={COLORS[i % COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip />
                    </PieChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>
            </div>
          </>
        )}
      </main>
    </ProtectedRoute>
  );
}
