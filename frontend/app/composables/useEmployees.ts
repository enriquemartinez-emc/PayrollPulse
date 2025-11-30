import type { Employee, RegisterEmployee } from "~/types";

export function useEmployees() {
  const employees = useState<Employee[]>("employees", () => []);
  const pageSize = ref(10);

  const { data, execute, status, page } = usePaginated<Employee>(
    "/employees",
    pageSize.value
  );

  async function fetchEmployees(refresh = false, employeesPageSize = 10) {
    if (status.value !== "idle" && !refresh) return;
    pageSize.value = employeesPageSize;
    await execute();
    employees.value = data.value.items || [];
  }

  async function registerEmployee(body: RegisterEmployee) {
    const { token } = useAuth();
    try {
      const newEmployee = await $fetch<Employee>("/employees", {
        method: "POST",
        body,
        baseURL: `${useRuntimeConfig().public.apiUrl}`,
        headers: {
          Authorization: `Bearer ${token.value}`,
        },
      });
      employees.value.unshift(newEmployee);
      return newEmployee;
    } catch (err: any) {
      console.log(err.message ?? "Failed to register employee");
      return null;
    }
  }

  return {
    employees,
    page,
    status,
    fetchEmployees,
    registerEmployee,
  };
}
