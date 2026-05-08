import { type ClassValue, clsx } from "clsx";
import { RequestCookie } from "next/dist/compiled/@edge-runtime/cookies";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5023";

export const getData = async (craftId: string, token: RequestCookie | undefined) => {
  const res = await fetch(`${API_BASE_URL}/api/crafts/${craftId}`, {
    headers: token?.value ? { Authorization: `Bearer ${token.value}` } : {},
    method: "GET",
    cache: "no-store"
  });

  if (!res.ok) throw new Error("Failed to fetch data");
  return res.json();
};

export const sleep = async (ms: number) => new Promise<void>((resolve) => setTimeout(resolve, ms));

function getClientToken() {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("cc_token");
}

export function setClientToken(token: string | null) {
  if (typeof window === "undefined") return;
  if (!token) localStorage.removeItem("cc_token");
  else localStorage.setItem("cc_token", token);
}

export const customFetch = async ({
  pathName,
  method = "GET",
  body
}: {
  pathName: string;
  method?: "GET" | "POST" | "PATCH" | "PUT" | "DELETE";
  body?: object;
}): Promise<any> => {
  const headers: Record<string, string> = {};
  const token = getClientToken();
  if (token) headers.Authorization = `Bearer ${token}`;
  if (body) headers["Content-Type"] = "application/json";

  const response = await fetch(`${API_BASE_URL}/${pathName}`, {
    method,
    cache: "no-store",
    headers,
    body: body ? JSON.stringify(body) : undefined
  });

  if (!response.ok) {
    let message = `HTTP ${response.status}`;
    try {
      const data = await response.json();
      message = data.error ?? message;
    } catch {
      // ignore
    }
    throw new Error(message);
  }

  if (response.status === 204) return null;
  return response.json();
};
