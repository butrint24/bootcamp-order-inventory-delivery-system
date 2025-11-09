"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
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

export default function AdminDeliveriesPage() {
  const [deliveries, setDeliveries] = useState<Delivery[]>([])
  const [loading, setLoading] = useState(true)
  const [statusFilter, setStatusFilter] = useState("all")

  const fetchDeliveries = async () => {
    try {
      const response = await apiClient.get("/api/Delivery")
      setDeliveries(response.data)
    } catch (error) {
      console.error("Failed to fetch deliveries:", error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchDeliveries()
  }, [])

  const filteredDeliveries =
    statusFilter === "all" ? deliveries : deliveries.filter((d) => d.status === statusFilter)

  return (
    <ProtectedRoute requiredRole="admin">
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Delivery Management</h1>
          <p className="text-muted-foreground">Manage and track all deliveries</p>
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
              <CardTitle>All Deliveries</CardTitle>
              <CardDescription>Total: {filteredDeliveries.length} deliveries</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b">
                      <th className="text-left py-2 px-4 font-medium">Order #</th>
                      <th className="text-left py-2 px-4 font-medium">Status</th>
                      <th className="text-left py-2 px-4 font-medium">Current Location</th>
                      <th className="text-left py-2 px-4 font-medium">Driver</th>
                      <th className="text-left py-2 px-4 font-medium">Est. Delivery</th>
                      <th className="text-left py-2 px-4 font-medium">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {filteredDeliveries.map((delivery) => (
                      <tr key={delivery.id} className="border-b hover:bg-muted/50">
                        <td className="py-2 px-4 font-medium">{delivery.orderNumber}</td>
                        <td className="py-2 px-4">
                          <span
                            className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(
                              delivery.status
                            )}`}
                          >
                            {delivery.status}
                          </span>
                        </td>
                        <td className="py-2 px-4 text-sm">{delivery.currentLocation || "-"}</td>
                        <td className="py-2 px-4 text-sm">{delivery.assignedDriver || "Unassigned"}</td>
                        <td className="py-2 px-4 text-sm text-muted-foreground">
                          {new Date(delivery.estimatedDeliveryDate).toLocaleDateString()}
                        </td>
                        <td className="py-2 px-4">
                          <Link href={`/admin/deliveries/${delivery.id}`}>
                            <Button size="sm" variant="outline">
                              View
                            </Button>
                          </Link>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
                {filteredDeliveries.length === 0 && (
                  <div className="text-center py-8 text-muted-foreground">No deliveries found</div>
                )}
              </div>
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
