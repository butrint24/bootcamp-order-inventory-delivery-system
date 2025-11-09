"use client";

import { useAuth } from "@/lib/auth-context";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import Link from "next/link";

export function Navbar() {
  const { user, logout, isAdmin } = useAuth();
  const router = useRouter();

  const handleLogout = () => {
    logout();
    router.push("/login");
  };

  if (!user) return null;

  return (
    <nav className="border-b border-border bg-card">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Left side: Logo and nav links */}
          <div className="flex items-center gap-8">
            <Link href="/" className="font-bold text-lg">
              OrderHub
            </Link>

            <div className="hidden md:flex items-center gap-6">
              {isAdmin ? (
                <>
                  <Link href="/admin/dashboard" className="text-sm hover:text-primary">
                    Dashboard
                  </Link>
                  <Link href="/admin/inventory" className="text-sm hover:text-primary">
                    Inventory
                  </Link>
                  <Link href="/admin/orders" className="text-sm hover:text-primary">
                    Orders
                  </Link>
                  <Link href="/admin/deliveries" className="text-sm hover:text-primary">
                    Deliveries
                  </Link>
                </>
              ) : (
                <>
                  <Link href="/dashboard" className="text-sm hover:text-primary">
                    Dashboard
                  </Link>
                  <Link href="/orders" className="text-sm hover:text-primary">
                    Orders
                  </Link>
                  <Link href="/deliveries" className="text-sm hover:text-primary">
                    Deliveries
                  </Link>
                </>
              )}
            </div>
          </div>

          {/* Right side: User info and logout */}
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
              <Avatar className="w-8 h-8">
                <AvatarFallback>{user.email[0].toUpperCase()}</AvatarFallback>
              </Avatar>
              <span className="text-sm text-muted-foreground">{user.email}</span>
            </div>
            <Button variant="ghost" size="sm" onClick={handleLogout}>
              Logout
            </Button>
          </div>
        </div>
      </div>
    </nav>
  );
}
