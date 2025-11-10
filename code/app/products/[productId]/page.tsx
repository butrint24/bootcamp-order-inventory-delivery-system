"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { Navbar } from "@/components/navbar";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { apiClient } from "@/lib/api-client";
import Link from "next/link";


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
        console.log("Fetched product:", res.data); // 👈 Add this line
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
          <div className="text-center py-8 text-gray-500">Loading product...</div>
        ) : error ? (
          <div className="text-center py-8 text-red-500">{error}</div>
        ) : !product ? (
          <div className="text-center py-8 text-gray-500">Product not found.</div>
        ) : (
          <Card className="shadow-md border border-blue-100 rounded-xl bg-white">
            <CardHeader>
              <CardTitle className="text-2xl text-blue-600 font-bold">{product.name}</CardTitle>
              {product.category && (
                <CardDescription className="text-gray-500">
                  Category: {product.category}
                </CardDescription>
              )}
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-xl font-semibold text-gray-900">Price: ${product.price.toFixed(2)}</p>
              {product.stock !== undefined && (
                <p className={`text-sm ${product.stock > 0 ? "text-green-600" : "text-red-500"}`}>
                  {product.stock > 0 ? `In stock: ${product.stock}` : "Out of stock"}
                </p>
              )}
              {product.description && (
                <p className="text-gray-700 leading-relaxed">{product.description}</p>
              )}

              <Button
                asChild
                className="mt-6 w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded-lg"
              >
                <Link href={`/orders/new?productId=${product.productId}`}>Order This Product</Link>
              </Button>
            </CardContent>
          </Card>
        )}
      </main>
    </>
  );
}
