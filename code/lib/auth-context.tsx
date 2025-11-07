"use client";

import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { apiClient } from "./api-client";

/* ---------- Types ---------- */
export type UserRole = "admin" | "user" | "Admin" | "User";

export interface User {
  id?: string;
  userId?: string;
  email: string;
  role?: UserRole;
  roles?: string[];
  name?: string;
  surname?: string;
}

export interface RegisterPayload {
  name: string;
  surname: string;
  email: string;
  password: string;
  tel?: string;
  dateOfBirth?: string; // yyyy-mm-dd
  address?: string;
  role: UserRole;
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<User>;
  register: (data: RegisterPayload) => Promise<User>;
  logout: () => void;
  isAdmin: boolean;
}

/* ---------- Helpers ---------- */
const STORAGE = { token: "access_token", refresh: "refresh_token", user: "user" } as const;

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const hasAdmin = (u: User | null) => {
  if (!u) return false;
  const arr = [
    ...(u.roles ?? []).map((r) => r.toLowerCase()),
    ...(u.role ? [u.role.toLowerCase()] : []),
  ];
  return arr.includes("admin");
};

// normalizo user.id pavarësisht emërtimit
function normalizeUser(raw: any) {
  if (!raw) return null;
  const id = raw.id ?? raw.userId ?? raw.UserId ?? null;
  return { ...raw, id };
}

function extractAuth(resData: any) {
  const user = resData?.user ?? resData?.User ?? null;

  const tokensObj = resData?.tokens ?? resData?.Tokens ?? {};
  const access =
    tokensObj?.accessToken ??
    tokensObj?.AccessToken ??
    resData?.token ??
    resData?.accessToken ??
    resData?.access_token ??
    null;

  const refresh =
    tokensObj?.refreshToken ??
    tokensObj?.RefreshToken ??
    resData?.refreshToken ??
    resData?.refresh_token ??
    null;

  return { token: access, refreshToken: refresh, user: normalizeUser(user) };
}

function normalizeDate(date: string | undefined) {
  if (!date) return undefined;
  try {
    return new Date(date).toISOString().slice(0, 10);
  } catch {
    return date;
  }
}

/* ---------- Provider ---------- */
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const t = localStorage.getItem(STORAGE.token) || localStorage.getItem("token");
    const u = localStorage.getItem(STORAGE.user);
    if (t) setToken(t);
    if (u) setUser(JSON.parse(u));
    setLoading(false);
  }, []);

  const LOGIN_PATH = "/api/Auth/login";
  const SIGNUP_PATH = "/api/Auth/signup";

  const persistAuth = (tok: string, refreshToken?: string | null, userObj: any = null) => {
    const u = normalizeUser(userObj);
    localStorage.setItem(STORAGE.token, tok);
    if (refreshToken) localStorage.setItem(STORAGE.refresh, refreshToken);
    if (u) localStorage.setItem(STORAGE.user, JSON.stringify(u));
    setToken(tok);
    if (u) setUser(u);
  };

  const login = async (email: string, password: string) => {
    const res = await apiClient.post(LOGIN_PATH, { email, password });
    const { token, refreshToken, user } = extractAuth(res.data);
    if (!token || !user) throw new Error("Invalid login response.");
    persistAuth(token!, refreshToken, user);
    return user as User;
  };

  const register = async (p: RegisterPayload) => {
    const backendRole = String(p.role).toLowerCase() === "admin" ? "Admin" : "User";

    const res = await apiClient.post(
      SIGNUP_PATH,
      {
        Name: p.name,
        Surname: p.surname,
        Email: p.email,
        Password: p.password,
        Tel: p.tel || null,
        DateOfBirth: normalizeDate(p.dateOfBirth) || null,
        Address: p.address || null,
        Role: backendRole,
      },
      { validateStatus: () => true }
    );

    const { token, refreshToken, user } = extractAuth(res.data ?? {});
    if (token && user) {
      persistAuth(token, refreshToken, user);
      return user as User;
    }

    if ([200, 201, 204].includes(res.status)) {
      const logged = await login(p.email, p.password);
      return logged;
    }

    const apiMsg = res.data?.message || res.data?.error || `Registration failed (${res.status})`;
    const err: any = new Error(apiMsg);
    err.response = { status: res.status, data: res.data };
    throw err;
  };

  const logout = () => {
    localStorage.removeItem(STORAGE.token);
    localStorage.removeItem(STORAGE.refresh);
    localStorage.removeItem(STORAGE.user);
    setToken(null);
    setUser(null);
  };

  const value = useMemo<AuthContextType>(
    () => ({ user, token, loading, login, register, logout, isAdmin: hasAdmin(user) }),
    [user, token, loading]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
