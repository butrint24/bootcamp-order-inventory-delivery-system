"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { useEffect, useState } from "react"
import { apiClient } from "@/lib/api-client"
import { useParams } from "next/navigation"
import Link from "next/link"

interface OrderDetail {
  id: string
  orderNumber: string
  status: string
  totalAmount: number
  createdAt: string
  items: Array<{ name: string; quantity: number; price: number }>
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

  useEffect(() => {
    const fetchOrder = async () => {
      try {
        const response = await apiClient.get(`/api/orders/${orderId}`)
        setOrder(response.data)
      } catch (error) {
        console.error("Failed to fetch order:", error)
      } finally {
        setLoading(false)
      }
    }

    fetchOrder()
  }, [orderId])

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-4xl mx-auto">
        <Link href="/orders">
          <Button variant="ghost" className="mb-4">
            ← Back to Orders
          </Button>
        </Link>

        {loading ? (
          <div className="text-center py-8">Loading order details...</div>
        ) : order ? (
          <>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Order Number</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-2xl font-bold">#{order.orderNumber}</p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Status</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-xl font-bold capitalize">{order.status}</p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Total Amount</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-2xl font-bold">${order.totalAmount.toFixed(2)}</p>
                </CardContent>
              </Card>
            </div>

            {/* Order Items */}
            <Card className="mb-6">
              <CardHeader>
                <CardTitle>Order Items</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  {order.items.map((item, idx) => (
                    <div key={idx} className="flex justify-between items-center p-3 border border-border rounded">
                      <div>
                        <p className="font-medium">{item.name}</p>
                        <p className="text-sm text-muted-foreground">Quantity: {item.quantity}</p>
                      </div>
                      <p className="font-medium">${(item.price * item.quantity).toFixed(2)}</p>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>

            {/* Delivery Information */}
            {order.delivery && (
              <Card>
                <CardHeader>
                  <CardTitle>Delivery Information</CardTitle>
                  <CardDescription>Track your delivery status</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    <div>
                      <label className="text-sm font-medium text-muted-foreground">Status</label>
                      <p className="text-lg font-semibold capitalize">{order.delivery.status}</p>
                    </div>
                    <div>
                      <label className="text-sm font-medium text-muted-foreground">Estimated Delivery Date</label>
                      <p className="text-lg font-semibold">
                        {new Date(order.delivery.estimatedDeliveryDate).toLocaleDateString()}
                      </p>
                    </div>
                    <div>
                      <label className="text-sm font-medium text-muted-foreground">ETA</label>
                      <p className="text-lg font-semibold">{order.delivery.eta}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
          </>
        ) : (
          <div className="text-center py-8 text-destructive">Failed to load order details</div>
        )}
      </main>
    </ProtectedRoute>
  )
}
