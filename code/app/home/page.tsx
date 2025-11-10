"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { Navbar } from "@/components/navbar";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
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
  const [error, setError] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const PAGE_SIZE = 8;

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        const res = await apiClient.get(`/api/Product?page=${page}&pageSize=${PAGE_SIZE}`);

        // Handle various response formats
        const data = res.data?.items || res.data?.data || res.data || [];
        setProducts(Array.isArray(data) ? data : []);
        setTotalPages(res.data?.totalPages || 1);
      } catch (e: any) {
        setError(e?.response?.data?.message || e?.message || "Failed to load products.");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [page]);

  return (
    <>
      <Navbar />
      <main className="p-6 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-blue-700">Our Products</h1>
          <p className="text-gray-500">Browse our catalog and find what you need</p>
        </div>

        {loading ? (
          <div className="text-center py-8 text-gray-500 animate-pulse">Loading products...</div>
        ) : error ? (
          <div className="text-center py-8 text-red-600">{error}</div>
        ) : products.length === 0 ? (
          <div className="text-center py-8 text-gray-500">No products found.</div>
        ) : (
          <>
            {/* Product Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 mb-8">
              {products.map((product) => (
                <Card
                  key={product.productId}
                  className="hover:shadow-lg transition-all duration-300 border-blue-100"
                >
                  <CardHeader>
                    <CardTitle className="text-lg font-semibold text-blue-700">
                      {product.name}
                    </CardTitle>
                    <CardDescription className="text-sm text-gray-500">
                      {product.description || "No description available"}
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <p className="font-bold text-xl text-blue-600 mb-3">
                      ${product.price.toFixed(2)}
                    </p>
                    <Button asChild className="w-full">
                      <Link href={`/products/${product.productId}`}>View Details</Link>
                    </Button>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Pagination */}
            <div className="flex justify-center items-center gap-4 mt-6">
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
          </>
        )}
      </main>
    </>
  );
}
