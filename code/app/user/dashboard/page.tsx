"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { ProtectedRoute } from "@/components/protected-route";
import { Navbar } from "@/components/navbar";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { apiClient } from "@/lib/api-client";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  LineChart,
  Line,
} from "recharts";

/* --------------------------------------------
   🔧 NDRYSHO KËTO PATH-E SIPAS API-së TËNDE
   Shembuj tipikë (vendosi saktë sipas backend-it tënd):
   - ORDERS_PATH:        "/api/order" ose "/api/orders"
   - DELIVERIES_PATH:    "/api/delivery" ose "/api/deliveries"
   - PRODUCTS_PATH:      "/api/inventory/products" ose "/api/product"
--------------------------------------------- */
const ORDERS_PATH = "/api/Order";
const DELIVERIES_PATH = "/api/Delivery";
const PRODUCTS_PATH = "/api/Product"; // ← kjo është rruga e saktë


/* --------------------------------------------
   Lloj-lloj formatesh mund të kthejë API-ja.
   Këtu i normalizojmë në forma të thjeshta.
--------------------------------------------- */
type AnyObj = Record<string, any>;

interface UserDashboardStats {
  totalOrders: number;
  activeDeliveries: number;
  completedOrders: number;
  recentOrders: Array<{ id: string; date: string; amount: number; status: string }>;
  deliveryTimeline: Array<{ day: string; deliveries: number }>;
}

function toArray(data: any): AnyObj[] {
  if (!data) return [];
  if (Array.isArray(data)) return data;
  // disa API kthejnë { items: [...] }
  if (Array.isArray((data as AnyObj).items)) return (data as AnyObj).items;
  // ose { data: [...] }
  if (Array.isArray((data as AnyObj).data)) return (data as AnyObj).data;
  return [];
}

function pickId(item: AnyObj) {
  return (
    String(item.id ?? item.orderId ?? item.deliveryId ?? item.productId ?? crypto.randomUUID())
  );
}
function pickDate(item: AnyObj) {
  const raw =
    item.date ??
    item.createdAt ??
    item.createdOn ??
    item.orderedAt ??
    item.created_date ??
    item.timestamp;
  if (!raw) return "";
  const d = new Date(raw);
  return isNaN(d.getTime()) ? String(raw) : d.toISOString().slice(0, 10);
}
function pickAmount(item: AnyObj) {
  // tentativë tipike: total, price, amount
  const val = item.total ?? item.amount ?? item.price ?? 0;
  const num = Number(val);
  return Number.isFinite(num) ? num : 0;
}
function pickStatus(item: AnyObj) {
  const s = item.status ?? item.state ?? item.deliveryStatus ?? item.orderStatus ?? "";
  return String(s || "");
}
function isDeliveryActive(item: AnyObj) {
  const s = pickStatus(item).toLowerCase();
  // përshtate sipas statuseve të tua reale
  return ["pending", "in_transit", "shipped", "active"].includes(s);
}
function isDeliveryCompleted(item: AnyObj) {
  const s = pickStatus(item).toLowerCase();
  return ["delivered", "completed", "done", "finished"].includes(s);
}

export default function UserDashboardPage() {
  const [stats, setStats] = useState<UserDashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState<string>("");

  useEffect(() => {
    const fetchAll = async () => {
      try {
        // Marrim të treja paralel
        const [ordersRes, deliveriesRes, productsRes] = await Promise.allSettled([
          apiClient.get(ORDERS_PATH),      // → /api/Order
          apiClient.get(DELIVERIES_PATH),  // → shih poshtë
          apiClient.get(PRODUCTS_PATH),    // → /api/Product
        ]);


        const orders = ordersRes.status === "fulfilled" ? toArray(ordersRes.value.data) : [];
        const deliveries = deliveriesRes.status === "fulfilled" ? toArray(deliveriesRes.value.data) : [];
        // products nuk e përdorim për statistika tani, ama e mbajmë për të ardhmen
        // const products = productsRes.status === "fulfilled" ? toArray(productsRes.value.data) : [];

        // Agregime në front
        const totalOrders = orders.length;

        const activeDeliveries = deliveries.filter(isDeliveryActive).length;
        const completedOrders =
          orders.filter((o) => {
            const s = pickStatus(o).toLowerCase();
            return ["completed", "delivered", "done", "finished"].includes(s);
          }).length ||
          deliveries.filter(isDeliveryCompleted).length;

        // Recent orders (marrim deri 6, sortim nga data me e re)
        const recentOrders = orders
          .map((o) => ({
            id: pickId(o),
            date: pickDate(o),
            amount: pickAmount(o),
            status: pickStatus(o),
          }))
          .sort((a, b) => (a.date < b.date ? 1 : -1))
          .slice(0, 6);

        // Delivery timeline (agregojmë numrin e dorëzimeve në ditë)
        const perDay = new Map<string, number>();
        deliveries.forEach((d) => {
          const day = pickDate(d) || "unknown";
          const inc = isDeliveryCompleted(d) ? 1 : 0;
          perDay.set(day, (perDay.get(day) ?? 0) + inc);
        });
        const deliveryTimeline = Array.from(perDay.entries())
          .map(([day, deliveries]) => ({ day, deliveries }))
          .sort((a, b) => (a.day < b.day ? -1 : 1))
          .slice(-14); // 2 javët e fundit, nëse ka të dhëna

        setStats({
          totalOrders,
          activeDeliveries,
          completedOrders,
          recentOrders,
          deliveryTimeline,
        });
      } catch (e: any) {
        setErr(e?.response?.data?.message || e?.message || "Failed to load");
        setStats(null);
      } finally {
        setLoading(false);
      }
    };

    fetchAll();
  }, []);

  // skeleton i thjeshtë kur s’ka të dhëna
  const hasNoData = useMemo(
    () =>
      !stats ||
      (stats.totalOrders === 0 &&
        stats.activeDeliveries === 0 &&
        stats.completedOrders === 0 &&
        stats.recentOrders.length === 0 &&
        stats.deliveryTimeline.length === 0),
    [stats]
  );

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Your Dashboard</h1>
          <p className="text-muted-foreground">Track your orders and deliveries</p>
        </div>

        {loading ? (
          <div className="text-center py-8">Loading dashboard...</div>
        ) : !stats ? (
          <div className="text-center py-8 text-destructive">
            Failed to load dashboard{err ? ` — ${err}` : ""}
          </div>
        ) : (
          <>
            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Total Orders</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold">{stats.totalOrders}</div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Active Deliveries</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold text-blue-600">{stats.activeDeliveries}</div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Completed Orders</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold text-green-600">{stats.completedOrders}</div>
                </CardContent>
              </Card>
            </div>

            {/* Quick Actions */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
              <Button asChild className="h-12">
                <Link href="/orders">View My Orders</Link>
              </Button>
              <Button asChild variant="outline" className="h-12 bg-transparent">
                <Link href="/deliveries">Track Deliveries</Link>
              </Button>
            </div>

            {/* Empty state */}
            {hasNoData ? (
              <Card className="mb-6">
                <CardHeader>
                  <CardTitle>No data yet</CardTitle>
                  <CardDescription>
                    Fill your first order or create a delivery to see analytics here.
                  </CardDescription>
                </CardHeader>
                <CardContent className="flex gap-3">
                  <Button asChild>
                    <Link href="/orders/new">Create Order</Link>
                  </Button>
                  <Button asChild variant="outline">
                    <Link href="/inventory">Add Product</Link>
                  </Button>
                </CardContent>
              </Card>
            ) : (
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
                <Card>
                  <CardHeader>
                    <CardTitle>Recent Orders</CardTitle>
                    <CardDescription>Your last 6 orders</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <ResponsiveContainer width="100%" height={300}>
                      <BarChart data={stats.recentOrders}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="date" />
                        <YAxis />
                        <Tooltip />
                        <Bar dataKey="amount" fill="#3b82f6" />
                      </BarChart>
                    </ResponsiveContainer>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle>Delivery Timeline</CardTitle>
                    <CardDescription>Order completion trend</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <ResponsiveContainer width="100%" height={300}>
                      <LineChart data={stats.deliveryTimeline}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="day" />
                        <YAxis />
                        <Tooltip />
                        <Line type="monotone" dataKey="deliveries" stroke="#10b981" />
                      </LineChart>
                    </ResponsiveContainer>
                  </CardContent>
                </Card>
              </div>
            )}

            {/* Recent Orders Preview */}
            <Card>
              <CardHeader>
                <CardTitle>Recent Orders Preview</CardTitle>
                <CardDescription>Your latest orders</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-2">
                  {stats.recentOrders.length === 0 ? (
                    <p className="text-muted-foreground text-center py-4">No orders yet</p>
                  ) : (
                    stats.recentOrders.slice(0, 5).map((order, idx) => (
                      <div key={idx} className="flex justify-between items-center p-3 rounded-lg border border-border">
                        <div>
                          <p className="font-medium">Order {order.id}</p>
                          <p className="text-sm text-muted-foreground">{order.date}</p>
                        </div>
                        <div className="text-right">
                          <p className="font-medium">${order.amount}</p>
                          <p className="text-sm text-muted-foreground">{order.status}</p>
                        </div>
                      </div>
                    ))
                  )}
                </div>
              </CardContent>
            </Card>
          </>
        )}
      </main>
    </ProtectedRoute>
  );
}
