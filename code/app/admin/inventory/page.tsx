"use client";

import { useState, useEffect } from "react";
import { ProtectedRoute } from "@/components/protected-route";
import { Navbar } from "@/components/navbar";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { apiClient } from "@/lib/api-client";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

/* --- Tipo sipas BACKEND-it --- */
type Product = {
  productId: string;
  name: string;
  origin: string;
  category: string;     // ruhet si TEXT/enum i serializuar si string
  price: number;
  stock: number;
};

type ProductForm = {
  name: string;
  origin: string;
  category: string;
  price: number | "";
  stock: number | "";
};

/* --- OPSIONET e kategorisë (përputhi me Shared.Enums.Category) --- */
const CATEGORIES = ["OTHER", "FOOD", "ELECTRONICS", "FURNITURE"] as const;

export default function InventoryPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState("");

  const [form, setForm] = useState<ProductForm>({
    name: "",
    origin: "",
    category: "OTHER",
    price: "",
    stock: "",
  });

  const fetchProducts = async () => {
    try {
      setError("");
      const res = await apiClient.get("/api/Product");
      // nëse kthen { items: [...] } ose thjesht [...]
      const data = res.data?.items ?? res.data ?? [];
      setProducts(
        data.map((p: any) => ({
          productId: p.productId ?? p.id,
          name: p.name,
          origin: p.origin,
          category: p.category,
          price: Number(p.price),
          stock: Number(p.stock),
        }))
      );
    } catch (e: any) {
      setError(e?.response?.data?.message || e.message || "Failed to fetch products");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const clearForm = () =>
    setForm({ name: "", origin: "", category: "OTHER", price: "", stock: "" });

  const handleDialogChange = (open: boolean) => {
    if (!open) {
      setEditingId(null);
      clearForm();
    }
    setDialogOpen(open);
  };

  const saveProduct = async () => {
    setError("");

    if (!form.name.trim()) return setError("Name është i detyrueshëm.");
    if (!form.origin.trim()) return setError("Origin është i detyrueshëm.");
    if (!form.category.trim()) return setError("Category është i detyrueshëm.");
    if (form.price === "" || Number(form.price) <= 0) return setError("Price duhet të jetë > 0.");
    if (form.stock === "" || Number(form.stock) < 0) return setError("Stock s’mund të jetë negativ.");

    // DTO i saktë për .NET (PascalCase)
    const payload = {
      Name: form.name,
      Stock: Number(form.stock),
      Origin: form.origin,
      Price: Number(form.price),
      Category: form.category, // p.sh. "OTHER"
    };

    try {
      if (editingId) {
        await apiClient.put(`/api/Product/${editingId}`, payload);
      } else {
        await apiClient.post(`/api/Product`, payload);
      }
      await fetchProducts();
      setDialogOpen(false);
      setEditingId(null);
      clearForm();
    } catch (e: any) {
      const modelErrors =
        e?.response?.data?.errors
          ? Object.values(e.response.data.errors).flat().join(" | ")
          : null;
      setError(modelErrors || e?.response?.data?.message || e.message || "Save failed");
    }
  };

  const onEdit = (p: Product) => {
    setEditingId(p.productId);
    setForm({
      name: p.name ?? "",
      origin: p.origin ?? "",
      category: p.category ?? "OTHER",
      price: p.price ?? "",
      stock: p.stock ?? "",
    });
    setDialogOpen(true);
  };

  const onDelete = async (id: string) => {
    if (!confirm("Delete this product?")) return;
    try {
      await apiClient.delete(`/api/Product/${id}`);
      await fetchProducts();
    } catch (e: any) {
      const modelErrors =
        e?.response?.data?.errors
          ? Object.values(e.response.data.errors).flat().join(" | ")
          : null;
      setError(modelErrors || e?.response?.data?.message || e.message || "Delete failed");
    }
  };

  return (
    <ProtectedRoute requiredRole="admin">
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold">Inventory Management</h1>
            <p className="text-muted-foreground">Manage products and stock</p>
          </div>
          <Dialog open={dialogOpen} onOpenChange={handleDialogChange}>
            <DialogTrigger asChild>
              <Button>{editingId ? "Edit Product" : "Add Product"}</Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader>
                <DialogTitle>{editingId ? "Edit Product" : "Add New Product"}</DialogTitle>
                <DialogDescription>Enter product details</DialogDescription>
              </DialogHeader>

              {error && <div className="bg-destructive/10 text-destructive text-sm p-3 rounded-md">{error}</div>}

              <div className="space-y-3">
                <div>
                  <label className="text-sm font-medium">Name</label>
                  <Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
                </div>

                <div>
                  <label className="text-sm font-medium">Origin</label>
                  <Input
                    value={form.origin}
                    onChange={(e) => setForm({ ...form, origin: e.target.value })}
                    placeholder="p.sh. Germany"
                  />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm font-medium">Category</label>
                    <select
                      className="w-full px-3 py-2 border rounded-md bg-background"
                      value={form.category}
                      onChange={(e) => setForm({ ...form, category: e.target.value })}
                    >
                      {CATEGORIES.map((c) => (
                        <option key={c} value={c}>{c}</option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="text-sm font-medium">Price</label>
                    <Input
                      type="number"
                      step="0.01"
                      value={form.price}
                      onChange={(e) => setForm({ ...form, price: e.target.value === "" ? "" : Number(e.target.value) })}
                    />
                  </div>
                </div>

                <div>
                  <label className="text-sm font-medium">Stock</label>
                  <Input
                    type="number"
                    value={form.stock}
                    onChange={(e) => setForm({ ...form, stock: e.target.value === "" ? "" : Number(e.target.value) })}
                  />
                </div>

                <Button onClick={saveProduct} className="w-full">
                  {editingId ? "Update Product" : "Create Product"}
                </Button>
              </div>
            </DialogContent>
          </Dialog>
        </div>

        {loading ? (
          <div className="text-center py-8">Loading...</div>
        ) : (
          <Card>
            <CardHeader>
              <CardTitle>Products</CardTitle>
              <CardDescription>All products in inventory</CardDescription>
            </CardHeader>
            <CardContent>
              {products.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">No products</div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead>
                      <tr className="border-b">
                        <th className="text-left py-2 px-4">Name</th>
                        <th className="text-left py-2 px-4">Origin</th>
                        <th className="text-left py-2 px-4">Category</th>
                        <th className="text-left py-2 px-4">Price</th>
                        <th className="text-left py-2 px-4">Stock</th>
                        <th className="text-left py-2 px-4">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {products.map((p) => (
                        <tr key={p.productId} className="border-b hover:bg-muted/50">
                          <td className="py-2 px-4">{p.name}</td>
                          <td className="py-2 px-4 text-muted-foreground">{p.origin}</td>
                          <td className="py-2 px-4 text-muted-foreground">{p.category}</td>
                          <td className="py-2 px-4">${p.price.toFixed(2)}</td>
                          <td className="py-2 px-4">{p.stock}</td>
                          <td className="py-2 px-4">
                            <div className="flex gap-2">
                              <Button size="sm" variant="outline" onClick={() => onEdit(p)}>
                                Edit
                              </Button>
                              <Button size="sm" variant="destructive" onClick={() => onDelete(p.productId)}>
                                Delete
                              </Button>
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </CardContent>
          </Card>
        )}
      </main>
    </ProtectedRoute>
  );
}
