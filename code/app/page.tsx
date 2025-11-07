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

interface CartItem extends Product {
  quantity: number;
}

export default function HomePage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [cart, setCart] = useState<CartItem[]>([]);
  const [isCartOpen, setIsCartOpen] = useState(false);
  const [isCheckoutOpen, setIsCheckoutOpen] = useState(false);

  const PAGE_SIZE = 8;

  // Load cart from localStorage
  useEffect(() => {
    const savedCart = localStorage.getItem("cart");
    if (savedCart) {
      setCart(JSON.parse(savedCart));
    }
  }, []);

  // Save cart to localStorage whenever it changes
  useEffect(() => {
    localStorage.setItem("cart", JSON.stringify(cart));
  }, [cart]);

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

  const addToCart = (product: Product) => {
    setCart((prev) => {
      const existing = prev.find((item) => item.productId === product.productId);
      if (existing) {
        return prev.map((item) =>
          item.productId === product.productId ? { ...item, quantity: item.quantity + 1 } : item
        );
      }
      return [...prev, { ...product, quantity: 1 }];
    });
  };

  const removeFromCart = (productId: string) => {
    setCart((prev) => prev.filter((item) => item.productId !== productId));
  };

  const updateQuantity = (productId: string, delta: number) => {
    setCart((prev) =>
      prev
        .map((item) =>
          item.productId === productId
            ? { ...item, quantity: Math.max(item.quantity + delta, 1) }
            : item
        )
        .filter((item) => item.quantity > 0)
    );
  };

  const totalPrice = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);

  const handleCheckout = () => setIsCheckoutOpen(true);

  const confirmCheckout = async () => {
    if (cart.length === 0) return;

    try {
      const payload = {
        itemsAndQuantities: cart.reduce<Record<string, number>>((acc, item) => {
          acc[item.productId] = item.quantity;
          return acc;
        }, {}),
      };

      await apiClient.post("/api/Order/buy-cart", payload);

      setCart([]);
      localStorage.removeItem("cart"); // clear localStorage after purchase
      setIsCheckoutOpen(false);
      setIsCartOpen(false);
      alert("Purchase successful!");
    } catch (e: any) {
      setIsCheckoutOpen(false);
      alert(e?.response?.data?.message || "Failed to complete purchase");
    }
  };

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
                    <div className="flex flex-col gap-2 mt-3">
                      <Button onClick={() => addToCart(product)}>Add to Cart</Button>
                      <Button asChild variant="outline">
                        <Link href={`/products/${product.productId}`}>View Details</Link>
                      </Button>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Pagination */}
            <div className="flex justify-center items-center gap-4 mt-6">
              <Button onClick={() => setPage((p) => Math.max(p - 1, 1))} disabled={page === 1}>
                Previous
              </Button>
              <span>
                Page {page} of {totalPages}
              </span>
              <Button onClick={() => setPage((p) => Math.min(p + 1, totalPages))} disabled={page === totalPages}>
                Next
              </Button>
            </div>

            {/* Small Cart Button */}
            {cart.length > 0 && (
              <div
                className="fixed bottom-4 right-4 w-60 bg-white border border-border p-4 shadow-lg rounded-lg cursor-pointer"
                onClick={() => setIsCartOpen(true)}
              >
                <h2 className="text-lg font-bold mb-2">Cart ({cart.length})</h2>
                <div className="flex justify-between font-bold">
                  <span>Total:</span>
                  <span>${totalPrice.toFixed(2)}</span>
                </div>
              </div>
            )}

            {/* Big Cart Modal */}
            {isCartOpen && (
              <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30">
                <div className="bg-white rounded-lg max-w-lg w-full p-6 shadow-lg">
                  <h2 className="text-xl font-bold mb-4">Your Cart</h2>

                  <div className="space-y-4 max-h-96 overflow-y-auto">
                    {cart.map((item) => (
                      <div key={item.productId} className="flex justify-between items-center">
                        <div>
                          <p className="font-medium">{item.name}</p>
                          <p className="text-sm text-muted-foreground">${item.price.toFixed(2)}</p>
                        </div>
                        <div className="flex items-center gap-2">
                          <Button size="sm" onClick={() => updateQuantity(item.productId, -1)}>
                            -
                          </Button>
                          <span>{item.quantity}</span>
                          <Button size="sm" onClick={() => updateQuantity(item.productId, 1)}>
                            +
                          </Button>
                          <Button size="sm" variant="destructive" onClick={() => removeFromCart(item.productId)}>
                            Remove
                          </Button>
                        </div>
                      </div>
                    ))}
                  </div>

                  <div className="mt-4 flex justify-between font-bold text-lg">
                    <span>Total:</span>
                    <span>${totalPrice.toFixed(2)}</span>
                  </div>

                  <div className="flex justify-end gap-2 mt-4">
                    <Button variant="outline" onClick={() => setIsCartOpen(false)}>
                      Close
                    </Button>
                    <Button onClick={handleCheckout}>Checkout</Button>
                  </div>
                </div>
              </div>
            )}

            {/* Checkout Confirmation Modal */}
            {isCheckoutOpen && (
              <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30">
                <div className="bg-white rounded-lg max-w-md w-full p-6 shadow-lg">
                  <h3 className="text-lg font-bold mb-2">Confirm Purchase</h3>
                  <p className="text-sm text-muted-foreground mb-4">
                    Are you sure you want to purchase these items?
                  </p>
                  <div className="flex justify-end gap-2">
                    <Button variant="outline" onClick={() => setIsCheckoutOpen(false)}>
                      Cancel
                    </Button>
                    <Button onClick={confirmCheckout}>Confirm</Button>
                  </div>
                </div>
              </div>
            )}
          </>
        )}
      </main>
    </>
  );
}
