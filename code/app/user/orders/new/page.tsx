"use client";

import { useSearchParams, useRouter } from "next/navigation";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { apiClient } from "@/lib/api-client";

export default function NewOrderPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const productId = searchParams.get("productId");
  const [product, setProduct] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [placing, setPlacing] = useState(false);

  // Fetch product details
  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const res = await apiClient.get(`/api/Product/${productId}`);
        setProduct(res.data);
      } catch (err) {
        console.error("Failed to fetch product:", err);
      } finally {
        setLoading(false);
      }
    };
    if (productId) fetchProduct();
  }, [productId]);

  const handleOrder = async () => {
    if (!productId) return alert("Missing product ID");
    try {
      setPlacing(true);
      await apiClient.post("/api/Order", { productId, quantity: 1 });
      alert("Order placed successfully!");
      router.push("/user/orders");
    } catch (err: any) {
      console.error("Order failed:", err);
      alert(err?.response?.data?.message || "Failed to place order");
    } finally {
      setPlacing(false);
    }
  };

  if (loading) return <div className="p-8 text-center text-gray-600">Loading product details...</div>;

  if (!product)
    return (
      <div className="p-8 text-center text-red-600">
        Product not found or invalid product ID.
      </div>
    );

  return (
    <main className="p-8 flex justify-center">
      <Card className="max-w-md w-full shadow-md border-blue-100">
        <CardHeader>
          <CardTitle className="text-xl font-semibold text-blue-700">
            Confirm Your Order
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <p>
            You are ordering: <span className="font-semibold">{product.name}</span>
          </p>
          <p>Price: ${product.price.toFixed(2)}</p>
          <div className="flex flex-col gap-3 mt-6">
            <Button onClick={handleOrder} disabled={placing}>
              {placing ? "Placing Order..." : "Confirm Order"}
            </Button>
            <Button
              variant="outline"
              onClick={() => router.push("/user/dashboard")}
            >
              Cancel
            </Button>
          </div>
        </CardContent>
      </Card>
    </main>
  );
}
