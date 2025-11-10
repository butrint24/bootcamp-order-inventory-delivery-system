"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";
import { useEffect, useState } from "react";
import Image from "next/image";

export default function AdminLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const router = useRouter();
  const { user, logout } = useAuth();
  const [pageTitle, setPageTitle] = useState("Dashboard");

  const navItems = [
    { name: "Dashboard", icon: "/icons/layout-dashboard.png", href: "/admin/dashboard" },
    { name: "Inventory", icon: "/icons/package.png", href: "/admin/inventory" },
    { name: "Orders", icon: "/icons/shopping-cart.png", href: "/admin/orders" },
    { name: "Deliveries", icon: "/icons/truck.png", href: "/admin/deliveries" },
  ];

  const activePage = navItems.find((item) => pathname === item.href);

  useEffect(() => {
    const last = pathname?.split("/").pop() || "dashboard";
    setPageTitle(last.charAt(0).toUpperCase() + last.slice(1).toLowerCase());
  }, [pathname]);

  useEffect(() => {
    document.title = `OrderHub Admin | ${pageTitle}`;
  }, [pageTitle]);

  useEffect(() => {
    const favicon = document.querySelector("link[rel='icon']");
    const newIcon = activePage?.icon || "/icons/layout-dashboard.png";
    if (favicon) {
      favicon.setAttribute("href", newIcon);
    } else {
      const link = document.createElement("link");
      link.rel = "icon";
      link.href = newIcon;
      document.head.appendChild(link);
    }
  }, [activePage]);

    const handleLogout = async () => {
    try {
        await logout();
        localStorage.clear();
        sessionStorage.clear();
        router.push("/"); // Redirects to home page
    } catch (error) {
        console.error("Logout failed:", error);
    }
    };

  return (
    <div className="flex min-h-screen bg-gray-50">
      {/* Sidebar */}
      <aside className="w-64 bg-white shadow-md flex flex-col">
        <div className="p-6 border-b text-center">
          <h1 className="text-2xl font-bold text-blue-600 leading-tight">
            OrderHub <br /> Admin
          </h1>
        </div>

        {/* Navigation */}
        <nav className="flex-1 p-4 space-y-2">
          {navItems.map((item) => {
            const isActive = pathname === item.href;
            return (
              <Link
                key={item.name}
                href={item.href}
                className={`flex items-center gap-3 px-4 py-2 rounded-lg font-medium transform transition-all duration-300 ease-in-out ${
                  isActive
                    ? "bg-blue-100 text-blue-700 scale-[1.03] shadow-sm"
                    : "text-gray-700 hover:text-blue-700 hover:bg-blue-50 hover:shadow-md hover:scale-[1.05]"
                }`}
              >
                <Image
                  src={item.icon}
                  alt={item.name}
                  width={18}
                  height={18}
                  className="object-contain"
                />
                <span>{item.name}</span>
              </Link>
            );
          })}
        </nav>
      </aside>

      {/* Main Content */}
      <main className="flex-1 flex flex-col">
        {/* Topbar */}
        <header className="flex justify-between items-center bg-white border-b px-8 py-5 shadow-sm sticky top-0 z-10">
          <div className="flex items-center gap-3">
            {activePage?.icon && (
              <Image
                src={activePage.icon}
                alt={pageTitle}
                width={22}
                height={22}
                className="object-contain opacity-90"
              />
            )}
            <h2 className="text-2xl font-bold text-blue-700 tracking-wide">
              {pageTitle}
            </h2>
          </div>

          <div className="flex items-center gap-4">
            <span className="text-gray-600 font-medium">
              {user?.email || "Unknown User"}
            </span>
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 text-red-600 border border-red-600 px-4 py-2 rounded-lg transition-all duration-300 hover:bg-red-600 hover:text-white hover:scale-[1.05] hover:shadow-md"
            >
              <Image
                src="/icons/log-out.png"
                alt="Logout"
                width={16}
                height={16}
                className="object-contain"
              />
              Logout
            </button>
          </div>
        </header>

        {/* Page Content */}
        <section className="p-8 flex-1 bg-gray-50">{children}</section>
      </main>
    </div>
  );
}
