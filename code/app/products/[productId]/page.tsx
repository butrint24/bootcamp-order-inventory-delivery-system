"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation"; // App Router hook
import { Navbar } from "@/components/navbar";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { apiClient } from "@/lib/api-client";

interface Product {
  productId: string;
  name: string;
  description?: string;
  price: number;
  stock?: number;
  category?: string;
}

export default function ProductPage() {
  const { productId } = useParams();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        setLoading(true);
        const res = await apiClient.get(`/api/Product/${productId}`);
        setProduct(res.data);
      } catch (e: any) {
        setError(e?.message || "Failed to load product");
      } finally {
        setLoading(false);
      }
    };

    if (productId) fetchProduct();
  }, [productId]);

  return (
    <>
      <Navbar />

      <main className="p-6 max-w-3xl mx-auto">
        {loading ? (
          <div className="text-center py-8">Loading product...</div>
        ) : error ? (
          <div className="text-center py-8 text-destructive">{error}</div>
        ) : !product ? (
          <div className="text-center py-8 text-muted-foreground">Product not found.</div>
        ) : (
          <Card className="mb-6">
            <CardHeader>
              <CardTitle className="text-2xl">{product.name}</CardTitle>
              {product.category && (
                <CardDescription>Category: {product.category}</CardDescription>
              )}
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-lg font-bold">Price: ${product.price.toFixed(2)}</p>
              {product.stock !== undefined && (
                <p className="text-sm text-muted-foreground">
                  {product.stock > 0 ? `In stock: ${product.stock}` : "Out of stock"}
                </p>
              )}
              {product.description && <p>{product.description}</p>}

              <Button asChild className="mt-4 w-full">
                <a href="/orders/new?productId={product.productId}">Order This Product</a>
              </Button>
            </CardContent>
          </Card>
        )}
      </main>
    </>
  );
}
