"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";

type Props = {
  children: React.ReactNode;
  requiredRole?: "admin" | "user";
};

export function ProtectedRoute({ children, requiredRole }: Props) {
  const { user, loading, isAdmin } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (loading) return;

    // jo i loguar → në login
    if (!user) {
      router.replace("/login");
      return;
    }

    // kontroll role
    if (requiredRole === "admin" && !isAdmin) {
      router.replace("/dashboard");
      return;
    }
    if (requiredRole === "user" && isAdmin) {
      router.replace("/admin/dashboard");
      return;
    }
  }, [user, loading, isAdmin, requiredRole, router]);

  // gjatë ngarkimit ose gjatë redirect-it, shfaq diçka
  if (loading || !user) {
    return <div className="p-6 text-center">Loading…</div>;
  }
  if (requiredRole === "admin" && !isAdmin) return null;
  if (requiredRole === "user" && isAdmin) return null;

  return <>{children}</>;
}
