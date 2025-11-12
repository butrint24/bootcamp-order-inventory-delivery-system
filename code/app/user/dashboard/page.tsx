"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { apiClient } from "@/lib/api-client";

interface Product {
  productId: string;
  name: string;
  description?: string;
  price: number;
}

export default function ProductLandingPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [addingToCart, setAddingToCart] = useState<string | null>(null);

  const router = useRouter();
  const PAGE_SIZE = 8;

  // ✅ Fetch Products
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        setError(null);

        const res = await apiClient.get(
          `/api/Product?page=${page}&pageSize=${PAGE_SIZE}`
        );
        const data = res.data?.items || res.data?.data || res.data || [];
        setProducts(Array.isArray(data) ? data : []);
        setTotalPages(res.data?.totalPages || 1);
      } catch (err: any) {
        console.error("Failed to load products:", err);
        setError(err?.response?.data?.message || "Failed to load products.");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [page]);

  // 🛒 Add to Cart (localStorage + Toast)
  const handleAddToCart = (productId: string) => {
    try {
      setAddingToCart(productId);

      const storedCart = JSON.parse(localStorage.getItem("cart") || "[]");
      const existing = storedCart.find(
        (item: any) => item.productId === productId
      );
      const product = products.find((p) => p.productId === productId);

      if (existing) {
        existing.quantity += 1;
      } else {
        storedCart.push({
          productId,
          name: product?.name,
          price: product?.price,
          quantity: 1,
        });
      }

      localStorage.setItem("cart", JSON.stringify(storedCart));

      // ✅ Show a nice green toast
      toast.success("Product added to your cart 🛒", {
        description: "Redirecting you to your cart...",
        duration: 2000,
      });

      // ⏳ Redirect to cart after 2 seconds
      setTimeout(() => {
        router.push("/user/cart");
      }, 2000);
    } catch (err) {
      console.error("Add to cart failed:", err);
      toast.error("Failed to add product ❌", {
        description: "Please try again later.",
      });
    } finally {
      setAddingToCart(null);
    }
  };

  return (
    <main className="p-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-blue-700">Our Products</h1>
        <p className="text-gray-500">
          Browse our catalog and find what you need
        </p>
      </div>

      {/* Loading / Error / Empty */}
      {loading ? (
        <div className="text-center py-12 text-gray-500 animate-pulse">
          Loading products...
        </div>
      ) : error ? (
        <div className="text-center py-12 text-red-600 font-medium">{error}</div>
      ) : products.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          No products found.
        </div>
      ) : (
        <>
          {/* Product Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 mb-8">
            {products.map((product) => (
              <Card
                key={product.productId}
                className="hover:shadow-lg transition-transform transform hover:scale-[1.02] border border-blue-100 rounded-xl"
              >
                <CardHeader>
                  <CardTitle className="text-lg font-semibold text-blue-700 truncate">
                    {product.name}
                  </CardTitle>
                  <CardDescription className="text-sm text-gray-500 line-clamp-2">
                    {product.description || "No description available"}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <p className="font-bold text-xl text-blue-600 mb-4">
                    ${product.price.toFixed(2)}
                  </p>

                  {/* Buttons */}
                  <div className="flex gap-2">
                    <Button asChild variant="outline" className="flex-1">
                      <Link href={`/user/products/${product.productId}`}>
                        View Details
                      </Link>
                    </Button>

                    <Button
                      onClick={() => handleAddToCart(product.productId)}
                      className="flex-1"
                      disabled={addingToCart === product.productId}
                    >
                      {addingToCart === product.productId
                        ? "Adding..."
                        : "Add to Cart"}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex justify-center items-center gap-4 mt-8">
              <Button
                onClick={() => setPage((p) => Math.max(p - 1, 1))}
                disabled={page === 1}
                variant="outline"
              >
                Previous
              </Button>
              <span className="text-gray-600 font-medium">
                Page {page} of {totalPages}
              </span>
              <Button
                onClick={() => setPage((p) => Math.min(p + 1, totalPages))}
                disabled={page === totalPages}
                variant="outline"
              >
                Next
              </Button>
            </div>
          )}
        </>
      )}
    </main>
  );
}
