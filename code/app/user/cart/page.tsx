"use client";

import { useEffect, useState } from "react";
import { apiClient } from "@/lib/api-client";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { toast } from "sonner"; // ✅ only toast imported

interface CartItem {
  productId: string;
  name: string;
  price: number;
  quantity: number;
  imageUrl?: string;
}

export default function CartPage() {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [checkoutLoading, setCheckoutLoading] = useState(false);
  const router = useRouter();

  // ✅ Load cart from localStorage once
  useEffect(() => {
    const stored = localStorage.getItem("cart");
    if (stored) setCartItems(JSON.parse(stored));
  }, []);

  // ✅ Remove one quantity or full item
  const removeItem = (productId: string) => {
    const updated = cartItems
      .map((item) => {
        if (item.productId === productId) {
          if (item.quantity > 1) {
            return { ...item, quantity: item.quantity - 1 }; // decrease by 1
          } else {
            return null; // remove completely if only 1
          }
        }
        return item;
      })
      .filter((item): item is CartItem => item !== null); // clean nulls

    setCartItems(updated);
    localStorage.setItem("cart", JSON.stringify(updated));

    toast.info("Removed one item from your cart", { duration: 2000 });
  };

  // ✅ Checkout handler
  const checkout = async () => {
    try {
      if (cartItems.length === 0) {
        toast.warning("Your cart is empty!");
        return;
      }

      setCheckoutLoading(true);

      // Build valid GUID dictionary
      const itemsAndQuantities: Record<string, number> = {};
      cartItems.forEach((item) => {
        if (
          item.productId &&
          /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/.test(
            item.productId
          )
        ) {
          itemsAndQuantities[item.productId] = item.quantity;
        } else {
          console.warn("❌ Skipping invalid productId:", item.productId);
        }
      });

      if (Object.keys(itemsAndQuantities).length === 0) {
        toast.error("No valid products found in your cart.");
        return;
      }

      // Send checkout request
      await apiClient.post("/api/Order/buy-cart", { itemsAndQuantities });

      // Clear cart
      localStorage.removeItem("cart");
      setCartItems([]);

      toast.success("✅ Order placed successfully! Redirecting...", {
        duration: 2000,
      });

      setTimeout(() => router.push("/user/orders"), 2000);
    } catch (err: any) {
      console.error("Checkout failed:", err);
      toast.error(err?.response?.data?.message || "Failed to place order.");
    } finally {
      setCheckoutLoading(false);
    }
  };

  // 🧮 Total calculation
  const total = cartItems.reduce(
    (sum, item) => sum + item.price * item.quantity,
    0
  );

  return (
    <main className="p-8">
      {/* ✅ Toaster removed – global one in layout handles it */}

      <h1 className="text-3xl font-bold text-blue-700 mb-6">Your Cart</h1>

      {cartItems.length === 0 ? (
        <div className="text-center text-gray-600 py-8">
          Your cart is empty.{" "}
          <Link href="/user/dashboard" className="text-blue-600 underline">
            Browse products
          </Link>
        </div>
      ) : (
        <>
          {/* 🛍 Cart Items */}
          <div className="grid gap-4">
            {cartItems.map((item) => (
              <Card
                key={item.productId}
                className="flex justify-between items-center p-4 shadow-sm"
              >
                <div className="flex items-center gap-4">
                  {item.imageUrl ? (
                    <Image
                      src={item.imageUrl}
                      alt={item.name}
                      width={64}
                      height={64}
                      className="rounded-md object-cover"
                    />
                  ) : (
                    <div className="w-16 h-16 bg-gray-200 rounded-md" />
                  )}

                  <div>
                    <h3 className="font-semibold text-lg">{item.name}</h3>
                    <p className="text-gray-500 text-sm">
                      ${item.price.toFixed(2)} × {item.quantity}
                    </p>
                    <p className="text-blue-600 font-medium">
                      ${(item.price * item.quantity).toFixed(2)}
                    </p>
                  </div>
                </div>
                <Button
                  variant="destructive"
                  onClick={() => removeItem(item.productId)}
                >
                  Remove
                </Button>
              </Card>
            ))}
          </div>

          {/* 🧾 Summary */}
          <Card className="mt-8 p-6">
            <CardHeader>
              <CardTitle className="text-2xl font-bold text-blue-700">
                Summary
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <p className="text-gray-700">Items: {cartItems.length}</p>
              <p className="text-lg font-semibold">Total: ${total.toFixed(2)}</p>
              <Button
                className="w-full mt-4"
                onClick={checkout}
                disabled={checkoutLoading}
              >
                {checkoutLoading ? "Placing Order..." : "Checkout"}
              </Button>
            </CardContent>
          </Card>
        </>
      )}
    </main>
  );
}
