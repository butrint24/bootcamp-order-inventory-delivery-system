"use client";

import { motion } from "framer-motion";
import Link from "next/link";
import Image from "next/image";

export default function Home() {
  return (
    <main className="relative min-h-screen flex flex-col bg-gradient-to-b from-blue-50 via-white to-white text-gray-800">
      <nav className="flex justify-between items-center px-8 py-4 shadow-sm bg-white/70 backdrop-blur-md sticky top-0 z-50 border-b">
        <div className="flex items-center gap-2">
          <Image src="/icons/layout-dashboard.png" alt="OrderHub Logo" width={32} height={32} />
          <h1 className="text-2xl font-bold text-blue-600 tracking-tight">
            Order Management
          </h1>
        </div>

        <div className="flex gap-4">
          <Link
            href="/login"
            className="px-5 py-2 rounded-lg text-blue-600 font-medium border border-blue-600 hover:bg-blue-600 hover:text-white transition-all duration-300 hover:scale-105 hover:shadow-md"
          >
            Login
          </Link>
          <Link
            href="/register"
            className="px-5 py-2 rounded-lg bg-blue-600 text-white font-medium hover:bg-blue-700 transition-all duration-300 hover:scale-105 hover:shadow-md"
          >
            Register
          </Link>
        </div>
      </nav>

      <motion.section
        initial={{ opacity: 0, y: 40 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.6 }}
        className="flex flex-col items-center justify-center flex-1 text-center px-6 md:px-16 py-12"
      >
        <h2 className="text-5xl md:text-6xl font-bold mb-6 text-blue-700 leading-tight">
          Welcome to the <span className="text-blue-500">OrderHub</span> System
        </h2>
        <p className="text-lg text-gray-600 mb-10 leading-relaxed max-w-2xl">
          Manage your products, orders, deliveries, and inventory in one smart
          platform — built for efficiency, control, and simplicity.
        </p>
        <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.98 }}>
          <Link
            href="/login"
            className="px-10 py-4 bg-blue-600 text-white text-lg font-semibold rounded-xl hover:bg-blue-700 shadow-md transition-all duration-300"
          >
            Get Started
          </Link>
        </motion.div>
      </motion.section>

      <section className="py-16 bg-white border-t">
        <div className="max-w-6xl mx-auto grid md:grid-cols-3 gap-10 px-8 text-center">
          <motion.div
            whileHover={{ scale: 1.05 }}
            transition={{ duration: 0.2 }}
            className="p-6 rounded-xl bg-blue-50 shadow-sm hover:shadow-md transition-all"
          >
            <h3 className="text-xl font-semibold text-blue-700">📦 Inventory</h3>
            <p className="text-gray-600 text-sm mt-2">
              Keep track of stock levels and product availability in real time.
            </p>
          </motion.div>

          <motion.div
            whileHover={{ scale: 1.05 }}
            transition={{ duration: 0.2 }}
            className="p-6 rounded-xl bg-blue-50 shadow-sm hover:shadow-md transition-all"
          >
            <h3 className="text-xl font-semibold text-blue-700">🛒 Orders</h3>
            <p className="text-gray-600 text-sm mt-2">
              Manage customer orders from placement to delivery efficiently.
            </p>
          </motion.div>

          <motion.div
            whileHover={{ scale: 1.05 }}
            transition={{ duration: 0.2 }}
            className="p-6 rounded-xl bg-blue-50 shadow-sm hover:shadow-md transition-all"
          >
            <h3 className="text-xl font-semibold text-blue-700">🚚 Deliveries</h3>
            <p className="text-gray-600 text-sm mt-2">
              Streamline the entire delivery process with automated tracking.
            </p>
          </motion.div>
        </div>
      </section>

      <footer className="text-center py-6 text-gray-500 text-sm border-t bg-white">
        © {new Date().getFullYear()} OrderHub. All rights reserved.
      </footer>
    </main>
  );
}
