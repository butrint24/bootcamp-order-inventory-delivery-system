import "./globals.css";
import Providers from "./providers";

export const metadata = {
  title: "Order Management System",
  description: "Manage products, orders, deliveries, and inventory efficiently.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <head>
        {/* favicon - make sure this file exists in /public */}
        <link rel="icon" href="/icons/layout-dashboard.png" type="image/png" />
      </head>
      <body className="bg-white text-gray-900 antialiased">
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
