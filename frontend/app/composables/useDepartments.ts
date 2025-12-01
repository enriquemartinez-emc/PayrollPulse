import type { Department } from "~/types";

export async function useDepartments() {
  const { data, status, error, refresh } = await useFetch<Department[]>(
    "/departments",
    {
      lazy: true,
      server: true,
      baseURL: `${useRuntimeConfig().public.apiUrl}`,
      headers: {
        Authorization: `Bearer ${useCookie("auth-token").value}`,
      },
    }
  );

  return {
    departments: data,
    status,
    error,
    refresh,
  };
}
