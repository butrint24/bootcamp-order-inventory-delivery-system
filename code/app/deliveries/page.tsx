"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useState, useEffect } from "react"
import { apiClient } from "@/lib/api-client"
import Link from "next/link"

interface Delivery {
  deliveryId: string
  orderId: string
  status: string
  eta?: string | null
}

export default function DeliveriesPage() {
  const [deliveries, setDeliveries] = useState<Delivery[]>([])
  const [loading, setLoading] = useState(true)
  const [statusFilter, setStatusFilter] = useState("all")

  const fetchDeliveries = async () => {
    try {
      const response = await apiClient.get("/api/Delivery/my-deliveries")
      setDeliveries(response.data)
    } catch (error) {
      console.error("Failed to fetch deliveries:", error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchDeliveries()
    const interval = setInterval(fetchDeliveries, 30000)
    return () => clearInterval(interval)
  }, [])

  const filteredDeliveries =
    statusFilter === "all"
      ? deliveries
      : deliveries.filter((d) => d.status.toLowerCase() === statusFilter.toLowerCase())

  return (
    <ProtectedRoute requiredRole="user">
      <Navbar />
      <main className="p-6 max-w-5xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Track Deliveries</h1>
          <p className="text-muted-foreground">Monitor your order deliveries in real-time</p>
        </div>

        {/* Status Filter */}
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
              <option value="PENDING">Pending</option>
              <option value="PROCESSING">Processing</option>
              <option value="ON_ROUTE">On Route</option>
              <option value="DELIVERED">Delivered</option>
              <option value="CANCELED">Canceled</option>
            </select>
          </CardContent>
        </Card>

        {/* Deliveries List */}
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
                    <Link
                      key={delivery.deliveryId}
                      href={`/deliveries/${delivery.deliveryId}`}
                      className="block"
                    >
                      <div className="p-4 border border-border rounded-lg hover:bg-muted/50 transition-colors cursor-pointer">
                        <div className="space-y-4">
                          <div>
                            <p className="text-sm text-muted-foreground mb-1">Order ID</p>
                            <p className="font-medium">{delivery.orderId}</p>
                          </div>

                          <div>
                            <p className="text-sm text-muted-foreground mb-1">Status</p>
                            <span
                              className={`block px-3 py-2 rounded text-center text-sm font-medium ${getStatusColor(
                                delivery.status
                              )}`}
                            >
                              {formatStatusText(delivery.status)}
                            </span>
                          </div>

                          <div>
                            <p className="text-sm text-muted-foreground mb-1">ETA</p>
                            <p className="font-medium">
                              {delivery.eta
                                ? new Date(delivery.eta).toLocaleDateString()
                                : "No ETA available"}
                            </p>
                          </div>
                        </div>
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
    PENDING: "bg-gray-100 text-gray-800",
    PROCESSING: "bg-blue-100 text-blue-800",
    ON_ROUTE: "bg-purple-100 text-purple-800",
    DELIVERED: "bg-green-100 text-green-800",
    CANCELED: "bg-red-100 text-red-800",
  }
  return colors[status] || "bg-gray-100 text-gray-800"
}

function formatStatusText(status: string): string {
  switch (status) {
    case "PENDING":
      return "Pending"
    case "PROCESSING":
      return "Processing"
    case "ON_ROUTE":
      return "On Route"
    case "DELIVERED":
      return "Delivered"
    case "CANCELED":
      return "Canceled"
    default:
      return status
  }
}
