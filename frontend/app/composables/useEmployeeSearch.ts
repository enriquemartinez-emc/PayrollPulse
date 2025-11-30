import { debounce } from "~/utils";

export function useEmployeeSearch() {
  const { token } = useAuth();
  const query = ref("");
  const debouncedQuery = ref("");

  watch(query, (val) => {
    if (!val) return;
    updateDebounced(val);
  });

  const { data, status, execute } = useFetch("/employees/search", {
    key: "employee-search",
    transform: (data: { id: string; fullName: string }[]) => {
      return data?.map((employee) => ({
        label: employee.fullName,
        value: employee.id,
      }));
    },
    query: { query: debouncedQuery },
    baseURL: `${useRuntimeConfig().public.apiUrl}`,
    headers: {
      Authorization: `Bearer ${token.value}`,
    },
    immediate: false,
  });

  const updateDebounced = debounce((val: string) => {
    debouncedQuery.value = val;
    execute();
  });

  return {
    employees: data,
    status,
    query,
  };
}
