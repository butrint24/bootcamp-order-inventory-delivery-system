"use client";

import {
  Dialog, DialogTrigger, DialogContent, DialogHeader, DialogTitle, DialogDescription,
} from "@/components/ui/dialog";
import { useState, useEffect, useMemo } from "react";
import { ProtectedRoute } from "@/components/protected-route";
import { Navbar } from "@/components/navbar";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Link from "next/link";
import { apiClient } from "@/lib/api-client";
import { useAuth } from "@/lib/auth-context";

/* -------- Types & helpers -------- */
type OrderRow = {
  orderId?: string;
  id?: string;
  orderNumber?: string;
  status?: string;
  price?: number;
  totalAmount?: number;
  createdAt?: string;
  items?: Array<{ productId?: string; quantity?: number; price?: number }>;
};

type Product = { productId: string; name: string; price: number };
type NewItem = { productId: string; quantity: number };

const ORDERS_PATH = "/api/Order";
const PRODUCTS_PATH = "/api/Product";

const getStatusColor = (status: string): string => {
  const colors: Record<string, string> = {
    pending: "bg-yellow-100 text-yellow-700",
    confirmed: "bg-blue-100 text-blue-700",
    shipped: "bg-purple-100 text-purple-700",
    delivered: "bg-green-100 text-green-700",
    cancelled: "bg-red-100 text-red-700",
    other: "bg-gray-100 text-gray-700",
  };
  return colors[status?.toLowerCase()] || "bg-gray-100 text-gray-700";
};

function normalizeOrders(raw: any): OrderRow[] {
  const list = Array.isArray(raw?.items) ? raw.items : Array.isArray(raw) ? raw : [];
  return list.map((o: any) => ({
    orderId: String(o.orderId ?? o.OrderId ?? o.id ?? ""),
    id: String(o.id ?? o.orderId ?? o.OrderId ?? ""),
    orderNumber: o.orderNumber ?? o.OrderNumber ?? undefined,
    status: String(o.status ?? o.Status ?? "OTHER"),
    price: Number(o.price ?? o.totalAmount ?? 0),
    totalAmount: Number(o.totalAmount ?? o.price ?? 0),
    createdAt: String(o.createdAt ?? o.CreatedAt ?? new Date().toISOString()),
    items: o.items ?? o.Items ?? [],
  }));
}

/* -------- CreateOrderBox -------- */
function CreateOrderBox({ onCreated }: { onCreated: () => void }) {
  const { user } = useAuth();
  const [products, setProducts] = useState<Product[]>([]);
  const [address, setAddress] = useState("");
  const [items, setItems] = useState<NewItem[]>([{ productId: "", quantity: 1 }]);
  const [err, setErr] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    apiClient
      .get(PRODUCTS_PATH, { params: { pageNumber: 1, pageSize: 100 } })
      .then((r) => {
        const list = (r.data?.items ?? r.data ?? []).map((p: any) => ({
          productId: String(p.productId ?? p.ProductId ?? p.id),
          name: String(p.name ?? p.Name),
          price: Number(p.price ?? p.Price ?? 0),
        }));
        setProducts(list);
      })
      .catch((e) => setErr(e?.response?.data?.message || e.message || "Failed to fetch products"));
  }, []);

  const total = useMemo(
    () => items.reduce((sum, it) => {
      const prod = products.find((p) => p.productId === it.productId);
      return sum + (prod?.price ?? 0) * (Number(it.quantity) || 0);
    }, 0),
    [items, products]
  );

  const addItem = () => setItems((x) => [...x, { productId: "", quantity: 1 }]);
  const removeItem = (i: number) => setItems((x) => x.filter((_, idx) => idx !== i));

  const createOrder = async () => {
    setErr("");

    if (!address.trim()) return setErr("Please enter a delivery address");
    if (items.some((it) => !it.productId)) return setErr("Select a product for each item");
    if (!user) return setErr("You must be logged in");

    const orderId = crypto.randomUUID();

    const payload = {
      OrderId: orderId,
      Address: address,
      Price: Number(total.toFixed(2)),
      Status: "PENDING",
      Items: items.map((it) => ({
        OrderItemId: crypto.randomUUID(),
        OrderId: orderId,
        ProductId: it.productId,
        Quantity: Number(it.quantity) || 1,
      })),
    };

    try {
      setLoading(true);
      await apiClient.post(ORDERS_PATH, payload);
      setAddress("");
      setItems([{ productId: "", quantity: 1 }]);
      onCreated();
    } catch (e: any) {
      const modelErrors =
        e?.response?.data?.errors ? Object.values(e.response.data.errors).flat().join(" | ") : null;
      setErr(modelErrors || e?.response?.data?.message || e.message || "Order creation failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-4">
      {err && <div className="bg-red-50 text-red-600 text-sm p-3 rounded border border-red-200">{err}</div>}

      <div>
        <label className="text-sm font-medium text-gray-700">Delivery Address</label>
        <Input value={address} onChange={(e) => setAddress(e.target.value)} placeholder="e.g., 123 Main St" />
      </div>

      <div className="space-y-3">
        {items.map((it, idx) => (
          <div key={idx} className="grid grid-cols-1 md:grid-cols-3 gap-3">
            <select
              className="w-full px-3 py-2 border border-gray-300 rounded-md bg-white focus:ring-2 focus:ring-blue-500"
              value={it.productId}
              onChange={(e) => setItems((x) => x.map((row, i) => (i === idx ? { ...row, productId: e.target.value } : row)))}
            >
              <option value="">— Select product —</option>
              {products.map((p) => (
                <option key={p.productId} value={p.productId}>
                  {p.name} — ${p.price.toFixed(2)}
                </option>
              ))}
            </select>

            <Input
              type="number"
              min={1}
              value={it.quantity}
              onChange={(e) => setItems((x) => x.map((row, i) => (i === idx ? { ...row, quantity: Number(e.target.value) } : row)))}
              placeholder="Qty"
            />

            <Button variant="outline" onClick={() => removeItem(idx)} disabled={items.length === 1}>
              Remove
            </Button>
          </div>
        ))}
        <Button variant="secondary" onClick={addItem}>
          + Add Item
        </Button>
      </div>

      <div className="flex items-center justify-between border-t pt-3">
        <div className="text-sm text-muted-foreground">Total</div>
        <div className="text-lg font-semibold text-blue-600">${total.toFixed(2)}</div>
      </div>

      <Button
        onClick={createOrder}
        disabled={loading || products.length === 0}
        className="w-full bg-blue-600 hover:bg-blue-700 text-white"
      >
        {loading ? "Creating..." : "Create Order"}
      </Button>
    </div>
  );
}

/* -------- OrdersPage -------- */
export default function OrdersPage() {
  const [orders, setOrders] = useState<OrderRow[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);

  const fetchOrders = async () => {
    try {
      const response = await apiClient.get(ORDERS_PATH);
      setOrders(normalizeOrders(response.data));
    } catch (error) {
      console.error("Failed to fetch orders:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchOrders(); }, []);

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold text-blue-600">My Orders</h1>
            <p className="text-gray-500">View and manage your orders</p>
          </div>
          <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
            <DialogTrigger asChild>
              <Button className="bg-blue-600 hover:bg-blue-700 text-white">+ New Order</Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader>
                <DialogTitle>Create New Order</DialogTitle>
                <DialogDescription>Fill in the details below to place an order.</DialogDescription>
              </DialogHeader>
              <CreateOrderBox onCreated={() => { fetchOrders(); setDialogOpen(false); }} />
            </DialogContent>
          </Dialog>
        </div>

        {loading ? (
          <div className="text-center py-8">Loading orders...</div>
        ) : (
          <Card>
            <CardHeader>
              <CardTitle>All Orders</CardTitle>
              <CardDescription>Total: {orders.length} orders</CardDescription>
            </CardHeader>
            <CardContent>
              {orders.length === 0 ? (
                <div className="text-center py-8 text-gray-500">No orders found</div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {orders.map((order) => {
                    const id = order.orderId || order.id || "";
                    const amount = Number(order.totalAmount ?? order.price ?? 0);
                    const created = order.createdAt ? new Date(order.createdAt).toLocaleDateString() : "";
                    const status = order.status ?? "OTHER";
                    return (
                      <Link key={id} href={`/orders/${id}`}>
                        <div className="p-4 border border-gray-200 rounded-xl bg-white shadow-sm hover:shadow-md transition-all">
                          <div className="flex justify-between items-start mb-2">
                            <div>
                              <p className="font-semibold text-blue-600">
                                Order #{order.orderNumber ?? (id ? id.slice(0, 8) : "—")}
                              </p>
                              <p className="text-sm text-gray-500">{created}</p>
                            </div>
                            <span className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(status)}`}>
                              {status}
                            </span>
                          </div>
                          <p className="text-sm text-gray-500 mb-2">
                            {(order.items?.length ?? 0)} item{(order.items?.length ?? 0) !== 1 ? "s" : ""}
                          </p>
                          <p className="font-bold text-lg text-gray-900">${amount.toFixed(2)}</p>
                        </div>
                      </Link>
                    );
                  })}
                </div>
              )}
            </CardContent>
          </Card>
        )}
      </main>
    </ProtectedRoute>
  );
}
