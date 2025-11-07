"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { useEffect, useState } from "react"
import { apiClient } from "@/lib/api-client"
import { useParams } from "next/navigation"
import Link from "next/link"

interface DeliveryDetail {
  id: string
  orderId: string
  orderNumber: string
  status: string
  estimatedDeliveryDate: string
  currentLocation: string
  eta: string
  assignedDriver: string
  driverPhone: string
  driverEmail: string
  driverRating: number
  trackingUpdates: Array<{
    timestamp: string
    location: string
    status: string
    message: string
  }>
}

export default function DeliveryDetailPage() {
  const params = useParams()
  const deliveryId = params.id as string
  const [delivery, setDelivery] = useState<DeliveryDetail | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchDelivery = async () => {
      try {
        const response = await apiClient.get(`/api/deliveries/${deliveryId}`)
        setDelivery(response.data)
      } catch (error) {
        console.error("Failed to fetch delivery:", error)
      } finally {
        setLoading(false)
      }
    }

    fetchDelivery()
    // Refresh delivery details every 15 seconds
    const interval = setInterval(fetchDelivery, 15000)
    return () => clearInterval(interval)
  }, [deliveryId])

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-4xl mx-auto">
        <Link href="/deliveries">
          <Button variant="ghost" className="mb-4">
            ← Back to Deliveries
          </Button>
        </Link>

        {loading ? (
          <div className="text-center py-8">Loading delivery details...</div>
        ) : delivery ? (
          <>
            {/* Status Overview */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Delivery Status</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-2xl font-bold capitalize">{delivery.status}</p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">Estimated Delivery</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-2xl font-bold">{new Date(delivery.estimatedDeliveryDate).toLocaleDateString()}</p>
                </CardContent>
              </Card>
            </div>

            {/* Current Location */}
            <Card className="mb-6">
              <CardHeader>
                <CardTitle>Current Location</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  <div>
                    <p className="text-sm text-muted-foreground mb-1">Location</p>
                    <p className="text-lg font-semibold">{delivery.currentLocation || "Not available"}</p>
                  </div>
                  {delivery.eta && (
                    <div>
                      <p className="text-sm text-muted-foreground mb-1">ETA</p>
                      <p className="text-lg font-semibold">{delivery.eta}</p>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>

            {/* Driver Information */}
            {delivery.assignedDriver && (
              <Card className="mb-6">
                <CardHeader>
                  <CardTitle>Driver Information</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div>
                      <p className="text-sm text-muted-foreground mb-1">Driver Name</p>
                      <p className="text-lg font-semibold">{delivery.assignedDriver}</p>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <p className="text-sm text-muted-foreground mb-1">Phone</p>
                        <p className="text-lg font-semibold">{delivery.driverPhone}</p>
                      </div>
                      <div>
                        <p className="text-sm text-muted-foreground mb-1">Email</p>
                        <p className="text-lg font-semibold">{delivery.driverEmail}</p>
                      </div>
                    </div>
                    <div>
                      <p className="text-sm text-muted-foreground mb-1">Driver Rating</p>
                      <p className="text-lg font-semibold">{"⭐".repeat(Math.round(delivery.driverRating))}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Tracking Timeline */}
            <Card>
              <CardHeader>
                <CardTitle>Tracking Timeline</CardTitle>
                <CardDescription>All delivery updates</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {delivery.trackingUpdates && delivery.trackingUpdates.length > 0 ? (
                    delivery.trackingUpdates.map((update, idx) => (
                      <div key={idx} className="flex gap-4 pb-4 border-b border-border last:border-0">
                        <div className="w-2 h-2 rounded-full bg-primary mt-2 flex-shrink-0" />
                        <div className="flex-1">
                          <p className="font-semibold">{update.message}</p>
                          <p className="text-sm text-muted-foreground">{update.location}</p>
                          <p className="text-sm text-muted-foreground">{new Date(update.timestamp).toLocaleString()}</p>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p className="text-muted-foreground">No tracking updates yet</p>
                  )}
                </div>
              </CardContent>
            </Card>
          </>
        ) : (
          <div className="text-center py-8 text-destructive">Failed to load delivery details</div>
        )}
      </main>
    </ProtectedRoute>
  )
}
