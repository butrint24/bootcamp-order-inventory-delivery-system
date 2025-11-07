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
        const data = res.data?.items || res.data || [];
        setProducts(data);
        setTotalPages(res.data?.totalPages || 1);
      } catch (e: any) {
        setError(e?.message || "Failed to load products");
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
          <h1 className="text-3xl font-bold">Products</h1>
          <p className="text-muted-foreground">Browse our catalog</p>
        </div>

        {loading ? (
          <div className="text-center py-8">Loading products...</div>
        ) : error ? (
          <div className="text-center py-8 text-destructive">{error}</div>
        ) : products.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">No products found.</div>
        ) : (
          <>
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 mb-6">
              {products.map((product) => (
                <Card key={product.productId}>
                  <CardHeader>
                    <CardTitle>{product.name}</CardTitle>
                    <CardDescription>{product.description || "No description"}</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <p className="font-bold text-lg">${product.price.toFixed(2)}</p>
                    <Button asChild className="mt-3 w-full">
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
              >
                Previous
              </Button>
              <span>
                Page {page} of {totalPages}
              </span>
              <Button
                onClick={() => setPage((p) => Math.min(p + 1, totalPages))}
                disabled={page === totalPages}
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
