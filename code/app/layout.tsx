import "./globals.css";
import Providers from "./providers";
import Link from "next/link";

export const metadata = {
  title: "Order Management System",
  description:
    "Manage products, orders, deliveries, and inventory efficiently.",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <head>
        {/* favicon - make sure this file exists in /public/icons/layout-dashboard.png */}
        <link rel="icon" href="/icons/layout-dashboard.png" type="image/png" />
      </head>
      <body className="bg-white text-gray-900 antialiased">
        <Providers>
          {/* ✅ Navigation bar */}
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

          {/* ✅ Page content */}
          <main className="max-w-6xl mx-auto px-6 py-10">{children}</main>
        </Providers>
      </body>
    </html>
  );
}
