"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useState, useEffect } from "react"
import { apiClient } from "@/lib/api-client"
import Link from "next/link"

interface Delivery {
  id: string
  orderId: string
  orderNumber: string
  status: string
  estimatedDeliveryDate: string
  currentLocation: string
  eta: string
  assignedDriver?: string
}

export default function DeliveriesPage() {
  const [deliveries, setDeliveries] = useState<Delivery[]>([])
  const [loading, setLoading] = useState(true)
  const [statusFilter, setStatusFilter] = useState("all")

  const fetchDeliveries = async () => {
    try {
      const response = await apiClient.get("/api/deliveries/my-deliveries")
      setDeliveries(response.data)
    } catch (error) {
      console.error("Failed to fetch deliveries:", error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchDeliveries()
    // Refresh deliveries every 30 seconds for real-time updates
    const interval = setInterval(fetchDeliveries, 30000)
    return () => clearInterval(interval)
  }, [])

  const filteredDeliveries = statusFilter === "all" ? deliveries : deliveries.filter((d) => d.status === statusFilter)

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Track Deliveries</h1>
          <p className="text-muted-foreground">Monitor your order deliveries in real-time</p>
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
              <option value="all">All Deliveries</option>
              <option value="pending">Pending</option>
              <option value="in-transit">In Transit</option>
              <option value="out-for-delivery">Out for Delivery</option>
              <option value="delivered">Delivered</option>
              <option value="failed">Failed</option>
            </select>
          </CardContent>
        </Card>

        {loading ? (
          <div className="text-center py-8">Loading deliveries...</div>
        ) : (
          <Card>
            <CardHeader>
              <CardTitle>Your Deliveries</CardTitle>
              <CardDescription>Total: {filteredDeliveries.length} deliveries</CardDescription>
            </CardHeader>
            <CardContent>
              {filteredDeliveries.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">No deliveries found</div>
              ) : (
                <div className="space-y-4">
                  {filteredDeliveries.map((delivery) => (
                    <Link key={delivery.id} href={`/deliveries/${delivery.id}`}>
                      <div className="p-4 border border-border rounded-lg hover:bg-muted/50 transition-colors">
                        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                          <div>
                            <p className="text-sm text-muted-foreground">Order #</p>
                            <p className="font-bold">{delivery.orderNumber}</p>
                          </div>
                          <div>
                            <p className="text-sm text-muted-foreground">Status</p>
                            <span
                              className={`inline-block px-2 py-1 rounded text-xs font-medium ${getStatusColor(delivery.status)}`}
                            >
                              {delivery.status}
                            </span>
                          </div>
                          <div>
                            <p className="text-sm text-muted-foreground">Current Location</p>
                            <p className="font-medium">{delivery.currentLocation || "Unknown"}</p>
                          </div>
                          <div>
                            <p className="text-sm text-muted-foreground">Estimated Delivery</p>
                            <p className="font-medium">
                              {new Date(delivery.estimatedDeliveryDate).toLocaleDateString()}
                            </p>
                          </div>
                        </div>
                        {delivery.eta && <div className="mt-2 text-sm text-muted-foreground">ETA: {delivery.eta}</div>}
                      </div>
                    </Link>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        )}
      </main>
    </ProtectedRoute>
  )
}

function getStatusColor(status: string): string {
  const colors: { [key: string]: string } = {
    pending: "bg-gray-100 text-gray-800",
    "in-transit": "bg-blue-100 text-blue-800",
    "out-for-delivery": "bg-purple-100 text-purple-800",
    delivered: "bg-green-100 text-green-800",
    failed: "bg-red-100 text-red-800",
  }
  return colors[status] || "bg-gray-100 text-gray-800"
}
