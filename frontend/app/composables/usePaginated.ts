import type { PaginatedList } from "~/types";

export function usePaginated<T>(url: string, pageSize = 10) {
  const page = ref(1);

  const { data, status, error, refresh, execute } = useFetch<PaginatedList<T>>(
    url,
    {
      default: () => ({
        items: [],
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      }),
      baseURL: `${useRuntimeConfig().public.apiUrl}`,
      headers: {
        Authorization: `Bearer ${useCookie("auth-token").value}`,
      },
      query: { page: page.value, pageSize },
      watch: [page],
      immediate: false,
      lazy: true,
      server: true,
    }
  );

  return { data, status, error, refresh, execute, page };
}
