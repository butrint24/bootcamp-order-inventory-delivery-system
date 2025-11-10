"use client"; // 👈 This line is required because we use useState/useEffect

import React, { useEffect, useState } from "react";

// ✅ Define the product type
interface Product {
  productId: string;
  name: string;
  description: string;
  price: number;
  category?: string;
  stockQuantity?: number;
}

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // ✅ Fetch products from your .NET API
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await fetch("http://localhost:7000/api/Product", {
          method: "GET",
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        setProducts(data);
      } catch (err: any) {
        console.error("Error fetching products:", err);
        setError("Failed to load products. Please check your backend.");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  // ✅ Display states
  if (loading) {
    return <div className="p-8 text-gray-500">Loading products...</div>;
  }

  if (error) {
    return <div className="p-8 text-red-600">{error}</div>;
  }

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold mb-6">Products</h1>

      {products.length === 0 ? (
        <p className="text-gray-500">No products found.</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {products.map((p) => (
            <div
              key={p.productId}
              className="border rounded-lg shadow hover:shadow-lg transition p-5"
            >
              <h2 className="text-lg font-semibold mb-2">{p.name}</h2>
              <p className="text-gray-600 mb-2">{p.description}</p>
              {p.category && (
                <p className="text-sm text-gray-500 mb-1">
                  Category: {p.category}
                </p>
              )}
              <p className="font-bold text-lg">${p.price}</p>
              {p.stockQuantity !== undefined && (
                <p className="text-sm text-gray-500 mt-1">
                  In Stock: {p.stockQuantity}
                </p>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
