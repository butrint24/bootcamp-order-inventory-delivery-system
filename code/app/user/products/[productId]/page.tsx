"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { apiClient } from "@/lib/api-client";
import { toast } from "sonner";

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
  const router = useRouter();
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

  const handleAddToCart = () => {
    if (!product) return;

    try {
      const storedCart = JSON.parse(localStorage.getItem("cart") || "[]");
      const existing = storedCart.find((item: any) => item.productId === product.productId);

      if (existing) {
        existing.quantity += 1;
      } else {
        storedCart.push({
          productId: product.productId,
          name: product.name,
          price: product.price,
          quantity: 1,
        });
      }

      localStorage.setItem("cart", JSON.stringify(storedCart));

      toast.success("✅ Product added to your cart!", {
        description: "Redirecting you to your cart...",
        duration: 2000,
      });

      setTimeout(() => {
        router.push("/user/cart");
      }, 2000);
    } catch {
      toast.error("❌ Failed to add product to cart", {
        description: "Please try again later.",
      });
    }
  };

  return (
    <main className="p-8 max-w-3xl mx-auto">
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
            <p className="text-xl font-semibold text-gray-900">
              Price: ${product.price.toFixed(2)}
            </p>
            {product.stock !== undefined && (
              <p
                className={`text-sm ${
                  product.stock > 0 ? "text-green-600" : "text-red-500"
                }`}
              >
                {product.stock > 0 ? `In stock: ${product.stock}` : "Out of stock"}
              </p>
            )}
            {product.description && (
              <p className="text-gray-700 leading-relaxed">{product.description}</p>
            )}

            <Button
              onClick={handleAddToCart}
              className="mt-6 w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded-lg"
            >
              Add to Cart
            </Button>
          </CardContent>
        </Card>
      )}
    </main>
  );
}
