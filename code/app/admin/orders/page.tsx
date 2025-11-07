"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { useEffect, useState } from "react"
import { apiClient } from "@/lib/api-client"
import { useAuth } from "@/lib/auth-context"

type UiOrder = {
  id: string
  orderNumber: string
  customerEmail: string
  status: string
  totalAmount: number
  createdAt: string
}

export default function AdminOrdersPage() {
  const { user } = useAuth()
  const [orders, setOrders] = useState<UiOrder[]>([])
  const [loading, setLoading] = useState(true)
  const [selectedOrder, setSelectedOrder] = useState<UiOrder | null>(null)
  const [statusFilter, setStatusFilter] = useState("all")

  // normalizim nga objektet e backend-it në UI
  const normalize = (raw: any): UiOrder => {
    const id = String(raw.orderId ?? raw.id ?? raw.OrderId ?? "")
    const email = String(raw.email ?? raw.userEmail ?? raw.customerEmail ?? raw.Email ?? "-")
    const status = String(raw.status ?? raw.Status ?? "OTHER")
    const createdAt = String(raw.createdAt ?? raw.CreatedAt ?? new Date().toISOString())
    // totali: provo fushat më të shpeshta, ose llogarite nga items nëse ekziston
    const price =
      Number(raw.price ?? raw.totalAmount ?? raw.TotalAmount ?? 0) ||
      (Array.isArray(raw.items)
        ? raw.items.reduce((s: number, it: any) => {
            const q = Number(it.quantity ?? it.Quantity ?? 0)
            const p = Number(it.price ?? it.Price ?? 0)
            return s + q * p
          }, 0)
        : 0)

    return {
      id,
      orderNumber: String(raw.orderNumber ?? raw.OrderNumber ?? id.slice(0, 8).toUpperCase()),
      customerEmail: email,
      status,
      totalAmount: Number(price),
      createdAt,
    }
  }

  const fetchOrders = async () => {
    try {
      setLoading(true)
      // NOTE: endpoint i saktë në backend-in tënd
      const res = await apiClient.get("/api/Order", { params: { pageNumber: 1, pageSize: 200 } })
      const data = res.data?.items ?? res.data ?? []
      setOrders(data.map((o: any) => normalize(o)))
    } catch (err) {
      console.error("Failed to fetch orders:", err)
      setOrders([])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchOrders()
  }, [])

  // PUT kërkon DTO të plotë -> merre porosinë e plotë, ndrysho statusin, ktheje mbrapa
  const toPgTs = (v?: any) => {
  const d = v ? new Date(v) : new Date();
  const pad = (n: number) => String(n).padStart(2, "0");
  // yyyy-MM-ddTHH:mm:ss (pa Z)
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
};

const updateOrderStatus = async (orderId: string, newStatus: string) => {
  try {
    // 1) lexo porosinë nga endpoint-i I SAKTË
    const { data: raw } = await apiClient.get(`/api/Order/${orderId}`);

    const id = String(raw.orderId ?? raw.OrderId ?? orderId);
    const created = raw.createdAt ?? raw.CreatedAt ?? new Date().toISOString();

    // 2) përgatit vlerat sipas DTO-ve
    let price = Number(raw.price ?? raw.Price ?? raw.totalAmount ?? raw.TotalAmount ?? 0);
    if (!isFinite(price) || price < 0.01) price = 0.01;

    const items = (raw.items ?? raw.Items ?? []).map((it: any) => ({
      OrderItemId: it.orderItemId ?? it.OrderItemId ?? crypto.randomUUID(),
      OrderId: id,
      ProductId: it.productId ?? it.ProductId,                 // DUHET GUID
      Quantity: Number(it.quantity ?? it.Quantity ?? 1),        // >= 1
      CreatedAt: toPgTs(it.createdAt ?? it.CreatedAt ?? created),
    }));

    const payload = {
      OrderId: id,
      UserId: raw.userId ?? raw.UserId ?? "00000000-0000-0000-0000-000000000000",
      Address: raw.address ?? raw.Address ?? "N/A",             // <= 250 chars
      Price: Number(price.toFixed(2)),
      Status: String(newStatus).toUpperCase(),                  // p.sh. CANCELLED
      CreatedAt: toPgTs(created),
      Items: items,
    };

    // 3) PUT te /api/Order/{id}
    await apiClient.put(`/api/Order/${id}`, payload);
    setSelectedOrder(null);
    fetchOrders();
  } catch (e: any) {
    // nxjerr ModelState errors që ta shohësh fushën që bie
    const errs = e?.response?.data?.errors
      ? Object.entries(e.response.data.errors)
          .map(([k, v]: any) => `${k}: ${(v as string[]).join(", ")}`)
          .join(" | ")
      : e?.response?.data?.message || e?.message;
    console.error("PUT /api/Order/{id} failed:", e?.response?.data ?? e);
    alert(errs || "Update failed");
  }
};



  const filtered = statusFilter === "all" ? orders : orders.filter((o) => o.status.toLowerCase() === statusFilter)

  return (
    <ProtectedRoute requiredRole="admin">
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Orders Management</h1>
          <p className="text-muted-foreground">View and manage all customer orders</p>
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
          <div className="text-center py-8">Loading orders...</div>
        ) : (
          <Card>
            <CardHeader>
              <CardTitle>All Orders</CardTitle>
              <CardDescription>Total: {filtered.length} orders</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b">
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
                      <tr key={o.id} className="border-b hover:bg-muted/50">
                        <td className="py-2 px-4 font-medium">{o.orderNumber}</td>
                        <td className="py-2 px-4">{o.customerEmail}</td>
                        <td className="py-2 px-4">
                          <span className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(o.status)}`}>
                            {o.status}
                          </span>
                        </td>
                        <td className="py-2 px-4">${o.totalAmount.toFixed(2)}</td>
                        <td className="py-2 px-4 text-sm text-muted-foreground">
                          {new Date(o.createdAt).toLocaleDateString()}
                        </td>
                        <td className="py-2 px-4">
                          <Button size="sm" variant="outline" onClick={() => setSelectedOrder(o)}>
                            View
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
                {filtered.length === 0 && (
                  <div className="text-center py-8 text-muted-foreground">No orders found</div>
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
                <DialogDescription>Order #{selectedOrder.orderNumber}</DialogDescription>
              </DialogHeader>

              <div className="space-y-4">
                <div>
                  <label className="text-sm font-medium">Customer Email</label>
                  <p>{selectedOrder.customerEmail}</p>
                </div>
                <div>
                  <label className="text-sm font-medium">Current Status</label>
                  <p>{selectedOrder.status}</p>
                </div>
                <div>
                  <label className="text-sm font-medium">Total Amount</label>
                  <p>${selectedOrder.totalAmount.toFixed(2)}</p>
                </div>
                <div>
                  <label className="text-sm font-medium">Update Status</label>
                  <select
                    defaultValue={selectedOrder.status.toLowerCase()}
                    onChange={(e) => updateOrderStatus(selectedOrder.id, e.target.value)}
                    className="w-full px-3 py-2 border border-input rounded-md bg-background"
                  >
                    <option value="pending">Pending</option>
                    <option value="confirmed">Confirmed</option>
                    <option value="shipped">Shipped</option>
                    <option value="delivered">Delivered</option>
                    <option value="cancelled">Cancelled</option>
                  </select>
                </div>
              </div>
            </DialogContent>
          </Dialog>
        )}
      </main>
    </ProtectedRoute>
  )
}

function getStatusColor(status: string): string {
  const s = status.toLowerCase()
  const colors: Record<string, string> = {
    pending: "bg-yellow-100 text-yellow-800",
    confirmed: "bg-blue-100 text-blue-800",
    shipped: "bg-purple-100 text-purple-800",
    delivered: "bg-green-100 text-green-800",
    cancelled: "bg-red-100 text-red-800",
  }
  return colors[s] || "bg-gray-100 text-gray-800"
}
