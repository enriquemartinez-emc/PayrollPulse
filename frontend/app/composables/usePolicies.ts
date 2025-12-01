import type { Policy } from "~/types";

export async function usePolicies() {
  const { data, status, execute } = await useFetch<Policy[]>("/policies", {
    lazy: true,
    server: true,
    baseURL: `${useRuntimeConfig().public.apiUrl}`,
    headers: {
      Authorization: `Bearer ${useCookie("auth-token").value}`,
    },
  });

  async function fetchPolicies(refresh = false) {
    if (status.value !== "idle" && !refresh) return;
    await execute();
  }

  return {
    policies: data,
    status,
    fetchPolicies,
  };
}
