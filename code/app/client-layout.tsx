"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

export default function ClientLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const hideNavbar = ["/login", "/register"].includes(pathname);

  return (
    <>
      {!hideNavbar && (
        <header className="border-b bg-white shadow-sm">
          <nav className="flex items-center justify-between max-w-6xl mx-auto py-4 px-6">
            <h1 className="text-xl font-bold">
              <Link href="/">OrderHub</Link>
            </h1>

            <div className="flex gap-6 text-gray-700 font-medium">
              <Link href="/user/dashboard" className="hover:text-black">
                Dashboard
              </Link>
              <Link href="/orders" className="hover:text-black">
                Orders
              </Link>
              <Link href="/deliveries" className="hover:text-black">
                Deliveries
              </Link>
              <Link href="/products" className="hover:text-black">
                Products
              </Link>
            </div>

            <div>
              <Link
                href="/login"
                className="text-sm text-gray-500 hover:text-black"
              >
                Logout
              </Link>
            </div>
          </nav>
        </header>
      )}

      <main className="max-w-6xl mx-auto px-6 py-10">{children}</main>
    </>
  );
}
