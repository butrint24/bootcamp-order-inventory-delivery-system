"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { useEffect, useState } from "react"
import { apiClient } from "@/lib/api-client"
import { useParams } from "next/navigation"
import Link from "next/link"

interface Product {
  id: string
  name: string
  price: number
  imageUrl?: string
}

interface OrderItem {
  productId: string
  quantity: number
  product?: Product
}

interface OrderDetail {
  orderId: string
  orderNumber: string
  status: string
  price: number
  createdAt: string
  items: OrderItem[]
  delivery?: {
    id: string
    status: string
    eta: string
    estimatedDeliveryDate: string
  }
}

export default function OrderDetailPage() {
  const params = useParams()
  const orderId = params.id as string

  const [order, setOrder] = useState<OrderDetail | null>(null)
  const [loading, setLoading] = useState(true)

  const [cancelDialogOpen, setCancelDialogOpen] = useState(false)
  const [cancelLoading, setCancelLoading] = useState(false)

  const [returnDialogOpen, setReturnDialogOpen] = useState(false)
  const [returnLoading, setReturnLoading] = useState(false)

  const fetchOrder = async () => {
    try {
      setLoading(true)
      const response = await apiClient.get(`/api/Order/${orderId}`)
      const orderData: OrderDetail = response.data

      const itemsWithProducts = await Promise.all(
        orderData.items.map(async (item) => {
          try {
            const productRes = await apiClient.get(`/api/Product/${item.productId}`)
            return { ...item, product: productRes.data }
          } catch (err) {
            console.error(`Failed to fetch product ${item.productId}:`, err)
            return item
          }
        })
      )

      setOrder({ ...orderData, items: itemsWithProducts })
    } catch (err) {
      console.error("Failed to fetch order:", err)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchOrder()
  }, [orderId])

  const handleCancel = async () => {
    if (!order) return
    try {
      setCancelLoading(true)
      await apiClient.post(`/api/Order/cancel-order/${order.orderId}`)
      setCancelDialogOpen(false)
      await fetchOrder()
    } catch (err: any) {
      console.error("Failed to cancel order:", err)
      alert(err?.response?.data?.message || "Failed to cancel order")
    } finally {
      setCancelLoading(false)
    }
  }

  const handleReturn = async () => {
    if (!order) return
    try {
      setReturnLoading(true)
      await apiClient.post(`/api/Order/return-order/${order.orderId}`)
      setReturnDialogOpen(false)
      await fetchOrder()
    } catch (err: any) {
      console.error("Failed to return order:", err)
      alert(err?.response?.data?.message || "Failed to return order")
    } finally {
      setReturnLoading(false)
    }
  }

  const canCancel = order && ["pending", "processing"].includes(order.status.toLowerCase())

  const canReturn =
    order &&
    ["completed", "delivered", "shipped"].includes(order.status.toLowerCase())

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case "pending":
        return "bg-yellow-100 text-yellow-800"
      case "processing":
        return "bg-blue-100 text-blue-800"
      case "completed":
      case "delivered":
        return "bg-green-100 text-green-800"
      case "shipped":
        return "bg-blue-200 text-blue-900"
      case "cancelled":
        return "bg-red-100 text-red-800"
      case "returned":
        return "bg-purple-100 text-purple-800"
      default:
        return "bg-gray-100 text-gray-800"
    }
  }

  return (
    <ProtectedRoute requiredRole="user">
      <main className="p-6 max-w-5xl mx-auto space-y-6">
        <Link href="/user/orders">
          <Button
            variant="ghost"
            className="text-blue-600 hover:bg-blue-100 hover:text-blue-800 transition-colors duration-200"
          >
            ← Back to Orders
          </Button>
        </Link>

        {loading ? (
          <div className="text-center py-16 text-lg">Loading order details...</div>
        ) : order ? (
          <>
            {/* Overview Cards */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <Card className="shadow-md rounded-lg">
                <CardHeader>
                  <CardTitle className="text-sm text-muted-foreground">Order ID</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-2xl font-semibold break-all">#{order.orderId}</p>
                </CardContent>
              </Card>

              <Card className="shadow-md rounded-lg">
                <CardHeader>
                  <CardTitle className="text-sm text-muted-foreground">Status</CardTitle>
                </CardHeader>
                <CardContent>
                  <p
                    className={`inline-block px-3 py-1 rounded-full font-semibold text-sm ${getStatusColor(
                      order.status
                    )}`}
                  >
                    {order.status}
                  </p>
                </CardContent>
              </Card>

              <Card className="shadow-md rounded-lg">
                <CardHeader>
                  <CardTitle className="text-sm text-muted-foreground">Order Price</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-2xl font-bold">${order.price.toFixed(2)}</p>
                </CardContent>
              </Card>
            </div>

            {/* Order Items */}
            <Card className="shadow-md rounded-lg">
              <CardHeader>
                <CardTitle>Order Items</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {order.items.map((item, idx) => (
                    <div
                      key={idx}
                      className="flex items-center justify-between p-4 border border-border rounded-lg hover:bg-muted/50 transition"
                    >
                      <div className="flex items-center gap-4">
                        {item.product?.imageUrl && (
                          <img
                            src={item.product.imageUrl}
                            alt={item.product.name}
                            className="w-20 h-20 object-cover rounded-lg"
                          />
                        )}
                        <div>
                          <p className="font-medium text-lg">{item.product?.name ?? "Unknown Product"}</p>
                          <p className="text-sm text-muted-foreground">
                            Quantity: {item.quantity}
                          </p>
                          <p className="text-sm text-muted-foreground">
                            Unit Price: ${item.product?.price.toFixed(2) ?? 0}
                          </p>
                        </div>
                      </div>
                      <p className="font-semibold text-lg">
                        ${((item.product?.price ?? 0) * item.quantity).toFixed(2)}
                      </p>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>

            {/* Cancel & Return Buttons */}
            <div className="flex justify-center gap-4">
              {/* Cancel */}
              <Dialog open={cancelDialogOpen} onOpenChange={setCancelDialogOpen}>
                <DialogTrigger asChild>
                  <Button
                    variant="destructive"
                    className="mt-4"
                    disabled={!canCancel}
                  >
                    Cancel Order
                  </Button>
                </DialogTrigger>
                <DialogContent className="max-w-sm">
                  <DialogHeader>
                    <DialogTitle>Cancel Order</DialogTitle>
                    <DialogDescription>
                      Are you sure you want to cancel this order? This action cannot be undone.
                    </DialogDescription>
                  </DialogHeader>
                  <div className="flex justify-end gap-3 mt-4">
                    <Button variant="outline" onClick={() => setCancelDialogOpen(false)}>
                      No, Keep
                    </Button>
                    <Button
                      variant="destructive"
                      onClick={handleCancel}
                      disabled={cancelLoading}
                    >
                      {cancelLoading ? "Cancelling..." : "Yes, Cancel"}
                    </Button>
                  </div>
                </DialogContent>
              </Dialog>

              {/* Return */}
              <Dialog open={returnDialogOpen} onOpenChange={setReturnDialogOpen}>
                <DialogTrigger asChild>
                  <Button
                    variant="secondary"
                    className="mt-4"
                    disabled={!canReturn}
                  >
                    Return Order
                  </Button>
                </DialogTrigger>
                <DialogContent className="max-w-sm">
                  <DialogHeader>
                    <DialogTitle>Return Order</DialogTitle>
                    <DialogDescription>
                      Are you sure you want to return this order? This action cannot be undone.
                    </DialogDescription>
                  </DialogHeader>
                  <div className="flex justify-end gap-3 mt-4">
                    <Button variant="outline" onClick={() => setReturnDialogOpen(false)}>
                      No, Keep
                    </Button>
                    <Button
                      variant="secondary"
                      onClick={handleReturn}
                      disabled={returnLoading}
                    >
                      {returnLoading ? "Returning..." : "Yes, Return"}
                    </Button>
                  </div>
                </DialogContent>
              </Dialog>
            </div>

            {/* Delivery Information */}
            {order.delivery && (
              <Card className="shadow-md rounded-lg">
                <CardHeader>
                  <CardTitle>Delivery Information</CardTitle>
                  <CardDescription>Track your delivery status</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm md:text-base">
                    <div className="p-3 bg-muted rounded-lg">
                      <p className="text-muted-foreground">Status</p>
                      <p className="font-semibold capitalize">{order.delivery.status}</p>
                    </div>
                    <div className="p-3 bg-muted rounded-lg">
                      <p className="text-muted-foreground">Estimated Delivery</p>
                      <p className="font-semibold">
                        {new Date(order.delivery.estimatedDeliveryDate).toLocaleDateString()}
                      </p>
                    </div>
                    <div className="p-3 bg-muted rounded-lg">
                      <p className="text-muted-foreground">ETA</p>
                      <p className="font-semibold">{order.delivery.eta}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
          </>
        ) : (
          <div className="text-center py-16 text-destructive">Failed to load order details</div>
        )}
      </main>
    </ProtectedRoute>
  )
}
