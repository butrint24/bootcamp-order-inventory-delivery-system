"use client";

import Link from "next/link";
import { useAuth } from "@/lib/auth-context";
import { Button } from "@/components/ui/button";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { useRouter } from "next/navigation";
import { LayoutGrid, Package, Truck } from "lucide-react";

export default function UserSidebar() {
  const { user, logout } = useAuth();
  const router = useRouter();

  if (!user) return null;

  const handleLogout = () => {
    logout();
    router.push("/login");
  };

  return (
    <aside className="w-64 h-screen bg-white border-r flex flex-col justify-between fixed">
      <div>
        {/* Header */}
        <div className="p-6 border-b">
          <h1 className="text-xl font-bold text-blue-700">OrderHub</h1>
          <p className="text-sm text-gray-500">User Panel</p>
        </div>

        {/* Navigation Links */}
        <nav className="mt-6 flex flex-col">
          <Link
            href="/user/dashboard"
            className="flex items-center gap-3 px-6 py-3 hover:bg-blue-50 text-gray-700"
          >
            <LayoutGrid className="w-5 h-5" />
            <span>Products</span>
          </Link>

          <Link
            href="/user/orders"
            className="flex items-center gap-3 px-6 py-3 hover:bg-blue-50 text-gray-700"
          >
            <Package className="w-5 h-5" />
            <span>Orders</span>
          </Link>

          <Link
            href="/user/deliveries"
            className="flex items-center gap-3 px-6 py-3 hover:bg-blue-50 text-gray-700"
          >
            <Truck className="w-5 h-5" />
            <span>Deliveries</span>
          </Link>
        </nav>
      </div>

      {/* Footer */}
      <div className="border-t p-4 flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Avatar className="w-8 h-8">
            <AvatarFallback>{user.email[0].toUpperCase()}</AvatarFallback>
          </Avatar>
          <span className="text-sm text-gray-600 truncate">{user.email}</span>
        </div>
        <Button variant="outline" size="sm" onClick={handleLogout}>
          Logout
        </Button>
      </div>
    </aside>
  );
}
