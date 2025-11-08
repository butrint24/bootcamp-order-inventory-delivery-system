import axios, { AxiosInstance, AxiosRequestConfig } from "axios";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:7000";
type RetriableAxiosRequestConfig = AxiosRequestConfig & { _retry?: boolean };

export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: false,
  headers: { "Content-Type": "application/json" },
});

apiClient.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("access_token") || localStorage.getItem("token");
    if (token) {
      config.headers = config.headers ?? {};
      (config.headers as any).Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as RetriableAxiosRequestConfig;

    if (!error.response || error.response.status !== 401 || originalRequest._retry) {
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      const refreshToken =
        (typeof window !== "undefined" &&
          (localStorage.getItem("refresh_token") || localStorage.getItem("refreshToken"))) || null;

      if (!refreshToken) throw new Error("No refresh token");

      const refreshUrl = `${API_BASE_URL}/api/Auth/refresh`; 
      const res = await axios.post(refreshUrl, { refreshToken });

      const access =
        res.data?.token ?? res.data?.accessToken ?? res.data?.access_token;
      const newRefresh =
        res.data?.refreshToken ?? res.data?.refresh_token ?? refreshToken;

      if (!access) throw new Error("Invalid refresh response");

      if (typeof window !== "undefined") {
        localStorage.setItem("access_token", access);
        localStorage.setItem("token", access);
        localStorage.setItem("refresh_token", newRefresh);
        localStorage.setItem("refreshToken", newRefresh);
      }

      originalRequest.headers = originalRequest.headers ?? {};
      (originalRequest.headers as any).Authorization = `Bearer ${access}`;

      return apiClient(originalRequest);
    } catch {
      if (typeof window !== "undefined") {
        localStorage.removeItem("access_token");
        localStorage.removeItem("token");
        localStorage.removeItem("refresh_token");
        localStorage.removeItem("refreshToken");
        window.location.href = "/login";
      }
      return Promise.reject(error);
    }
  }
);
