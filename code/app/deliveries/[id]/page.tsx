"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { useEffect, useState } from "react"
import { apiClient } from "@/lib/api-client"
import { useParams } from "next/navigation"
import Link from "next/link"

interface DeliveryDetail {
  deliveryId: string
  orderId: string
  status: string
  eta?: string | null
  createdAt: string
}

export default function DeliveryDetailPage() {
  const params = useParams()
  const deliveryId = params.id as string
  const [delivery, setDelivery] = useState<DeliveryDetail | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchDelivery = async () => {
      try {
        const response = await apiClient.get(`/api/Delivery/${deliveryId}`)
        setDelivery(response.data)
      } catch (error) {
        console.error("Failed to fetch delivery:", error)
      } finally {
        setLoading(false)
      }
    }

    fetchDelivery()
    const interval = setInterval(fetchDelivery, 15000)
    return () => clearInterval(interval)
  }, [deliveryId])

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-2xl mx-auto">
        <Link href="/deliveries">
          <Button variant="ghost" className="mb-6">
            ← Back to Deliveries
          </Button>
        </Link>

        {loading ? (
          <div className="text-center py-8">Loading delivery details...</div>
        ) : delivery ? (
          <Card className="p-8 shadow-lg">
            <CardHeader className="mb-6">
              <CardTitle className="text-2xl font-bold">Delivery Details</CardTitle>
            </CardHeader>
            <CardContent className="space-y-6 text-lg">
              <div className="p-4 bg-muted/10 rounded-lg">
                <p className="text-sm text-muted-foreground mb-1">Order ID</p>
                <p className="font-semibold">{delivery.orderId}</p>
              </div>

              <div className="p-4 bg-muted/10 rounded-lg">
                <p className="text-sm text-muted-foreground mb-1">Status</p>
                <span
                  className={`inline-block px-3 py-2 rounded text-base font-medium ${getStatusColor(
                    delivery.status
                  )}`}
                >
                  {delivery.status}
                </span>
              </div>

              <div className="p-4 bg-muted/10 rounded-lg">
                <p className="text-sm text-muted-foreground mb-1">ETA</p>
                <p className="font-semibold">
                  {delivery.eta ? new Date(delivery.eta).toLocaleDateString() : "No ETA available"}
                </p>
              </div>

              <div className="p-4 bg-muted/10 rounded-lg">
                <p className="text-sm text-muted-foreground mb-1">Created At</p>
                <p className="font-semibold">{new Date(delivery.createdAt).toLocaleDateString()}</p>
              </div>
            </CardContent>
          </Card>
        ) : (
          <div className="text-center py-8 text-destructive">Failed to load delivery details</div>
        )}
      </main>
    </ProtectedRoute>
  )
}

function getStatusColor(status: string): string {
  const colors: { [key: string]: string } = {
    pending: "bg-gray-100 text-gray-800",
    intransit: "bg-blue-100 text-blue-800",
    delivered: "bg-green-100 text-green-800",
    cancelled: "bg-red-100 text-red-800",
  }
  return colors[status.toLowerCase()] || "bg-gray-100 text-gray-800"
}
