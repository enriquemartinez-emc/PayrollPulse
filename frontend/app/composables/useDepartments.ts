import type { Department } from "~/types";

export async function useDepartments() {
  const { token } = useAuth();

  const { data, status, error, refresh } = await useFetch<Department[]>(
    "/departments",
    {
      lazy: true,
      server: true,
      baseURL: `${useRuntimeConfig().public.apiUrl}`,
      headers: {
        Authorization: `Bearer ${token.value}`,
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
