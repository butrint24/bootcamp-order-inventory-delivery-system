"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { Navbar } from "@/components/navbar"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { useState, useEffect } from "react"
import { apiClient } from "@/lib/api-client"
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"

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
  const [selectedDelivery, setSelectedDelivery] = useState<Delivery | null>(null)
  const [statusFilter, setStatusFilter] = useState("all")
  const [updateData, setUpdateData] = useState({
    status: "",
    currentLocation: "",
    eta: "",
    assignedDriver: "",
  })

  const fetchDeliveries = async () => {
    try {
      const response = await apiClient.get("/api/deliveries")
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

  const handleUpdateDelivery = async () => {
    if (!selectedDelivery) return
    try {
      await apiClient.put(`/api/deliveries/${selectedDelivery.id}`, updateData)
      fetchDeliveries()
      setSelectedDelivery(null)
      setUpdateData({ status: "", currentLocation: "", eta: "", assignedDriver: "" })
    } catch (error) {
      console.error("Failed to update delivery:", error)
    }
  }

  const handleSelectDelivery = (delivery: Delivery) => {
    setSelectedDelivery(delivery)
    setUpdateData({
      status: delivery.status,
      currentLocation: delivery.currentLocation,
      eta: delivery.eta,
      assignedDriver: delivery.assignedDriver || "",
    })
  }

  const filteredDeliveries = statusFilter === "all" ? deliveries : deliveries.filter((d) => d.status === statusFilter)

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
                          <span className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(delivery.status)}`}>
                            {delivery.status}
                          </span>
                        </td>
                        <td className="py-2 px-4 text-sm">{delivery.currentLocation || "-"}</td>
                        <td className="py-2 px-4 text-sm">{delivery.assignedDriver || "Unassigned"}</td>
                        <td className="py-2 px-4 text-sm text-muted-foreground">
                          {new Date(delivery.estimatedDeliveryDate).toLocaleDateString()}
                        </td>
                        <td className="py-2 px-4">
                          <Button size="sm" variant="outline" onClick={() => handleSelectDelivery(delivery)}>
                            Update
                          </Button>
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

        {selectedDelivery && (
          <Dialog open={!!selectedDelivery} onOpenChange={() => setSelectedDelivery(null)}>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Update Delivery</DialogTitle>
                <DialogDescription>Order #{selectedDelivery.orderNumber}</DialogDescription>
              </DialogHeader>
              <div className="space-y-4">
                <div>
                  <label className="text-sm font-medium">Status</label>
                  <select
                    value={updateData.status}
                    onChange={(e) => setUpdateData({ ...updateData, status: e.target.value })}
                    className="w-full px-3 py-2 border border-input rounded-md bg-background"
                  >
                    <option value="pending">Pending</option>
                    <option value="in-transit">In Transit</option>
                    <option value="out-for-delivery">Out for Delivery</option>
                    <option value="delivered">Delivered</option>
                    <option value="failed">Failed</option>
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium">Current Location</label>
                  <Input
                    value={updateData.currentLocation}
                    onChange={(e) => setUpdateData({ ...updateData, currentLocation: e.target.value })}
                    placeholder="e.g., Distribution Center, In Transit"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">ETA</label>
                  <Input
                    value={updateData.eta}
                    onChange={(e) => setUpdateData({ ...updateData, eta: e.target.value })}
                    placeholder="e.g., 2:00 PM"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Assigned Driver</label>
                  <Input
                    value={updateData.assignedDriver}
                    onChange={(e) => setUpdateData({ ...updateData, assignedDriver: e.target.value })}
                    placeholder="Driver name"
                  />
                </div>
                <Button onClick={handleUpdateDelivery} className="w-full">
                  Update Delivery
                </Button>
              </div>
            </DialogContent>
          </Dialog>
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
