"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useEffect, useState } from "react"
import { apiClient } from "@/lib/api-client"
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from "recharts"

type Stat = {
  totalOrders: number
  pendingDeliveries: number
  lowStockItems: number
  totalInventoryValue: number
  ordersPerDay: { day: string; count: number }[]
  statusDistribution: { name: string; value: number }[]
}

export default function AdminDashboardPage() {
  const [stats, setStats] = useState<Stat | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
  const fetchStats = async () => {
    try {
      // përdor routes ekzakte të controller-ave të tu .NET
      const results = await Promise.allSettled([
        apiClient.get("/api/Order",    { params: { pageNumber: 1, pageSize: 200 } }),
        apiClient.get("/api/Delivery", { params: { pageNumber: 1, pageSize: 200 } }),
        apiClient.get("/api/Product"), // pa params
      ]);

      const pick = (i: number) => {
        const r = results[i];
        if (r.status === "fulfilled") return r.value.data?.items ?? r.value.data ?? [];
        // log-o se cili ra dhe pse
        const err: any = (r as any).reason;
        console.warn(["Order","Delivery","Product"][i], "failed:", err?.response?.status, err?.response?.data || err?.message);
        return []; // vazhdo me bosh
      };

      const orders    = pick(0);
      const deliveries= pick(1);
      const products  = pick(2);

      // --- llogaritjet ---
      const totalOrders = orders.length;

      const pendingDeliveries = deliveries.filter((d: any) => {
        const s = String(d.status ?? d.Status ?? "").toLowerCase();
        return s && s !== "delivered";
      }).length;

      const lowStockItems = products.filter((p: any) => {
        const qty = Number(p.quantity ?? p.stock ?? p.stockQuantity ?? 0);
        const reorder = Number(p.reorderLevel ?? p.minStock ?? p.reorder ?? 0);
        return reorder > 0 && qty < reorder;
      }).length;

      const totalInventoryValue = products.reduce((sum: number, p: any) => {
        const price = Number(p.price ?? p.unitPrice ?? 0);
        const qty   = Number(p.quantity ?? p.stock ?? p.stockQuantity ?? 0);
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
        .sort((a,b) => a[0].localeCompare(b[0]))
        .slice(-7)
        .map(([day, count]) => ({ day, count }));

      const statusMap = new Map<string, number>();
      orders.forEach((o: any) => {
        const s = String(o.status ?? o.Status ?? "OTHER").toUpperCase();
        statusMap.set(s, (statusMap.get(s) ?? 0) + 1);
      });
      const statusDistribution = Array.from(statusMap.entries()).map(([name, value]) => ({ name, value }));

      setStats({
        totalOrders,
        pendingDeliveries,
        lowStockItems,
        totalInventoryValue: Number(totalInventoryValue.toFixed(2)),
        ordersPerDay,
        statusDistribution,
      });
    } catch (err) {
      console.error("Admin stats fatal:", err);
      setStats(null);
    } finally {
      setLoading(false);
    }
  };

  fetchStats();
}, []);


  return (
    <ProtectedRoute requiredRole="admin">
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Admin Dashboard</h1>
          <p className="text-muted-foreground">Manage inventory, orders, and deliveries</p>
        </div>

        {loading ? (
          <div className="text-center py-8">Loading dashboard...</div>
        ) : !stats ? (
          <div className="text-center py-8 text-destructive">Failed to load dashboard</div>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Total Orders</CardTitle>
                </CardHeader>
                <CardContent><div className="text-2xl font-bold">{stats.totalOrders}</div></CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Pending Deliveries</CardTitle>
                </CardHeader>
                <CardContent><div className="text-2xl font-bold text-amber-600">{stats.pendingDeliveries}</div></CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Low Stock Items</CardTitle>
                </CardHeader>
                <CardContent><div className="text-2xl font-bold text-red-600">{stats.lowStockItems}</div></CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Inventory Value</CardTitle>
                </CardHeader>
                <CardContent><div className="text-2xl font-bold">${stats.totalInventoryValue}</div></CardContent>
              </Card>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <Card>
                <CardHeader>
                  <CardTitle>Orders Per Day</CardTitle>
                  <CardDescription>Last 7 days</CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={stats.ordersPerDay}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="day" />
                      <YAxis allowDecimals={false} />
                      <Tooltip />
                      <Bar dataKey="count" fill="#3b82f6" />
                    </BarChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Order Status Distribution</CardTitle>
                  <CardDescription>Current status breakdown</CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <PieChart>
                      <Pie data={stats.statusDistribution} cx="50%" cy="50%" dataKey="value" outerRadius={90}
                           label={({ name, value }) => `${name}: ${value}`} >
                        {stats.statusDistribution.map((_, i) => <Cell key={i} />)}
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
  )
}
