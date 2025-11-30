import type { Payslip } from "~/types";

export async function usePayslips() {
  const employeeId = useAuth().user.value?.employeeId;
  const payslips = useState<Payslip[]>(
    `employee-payslips-${employeeId}`,
    () => []
  );

  const { token } = useAuth();

  const { data, status, error, execute } = await useFetch<Payslip[]>(
    `/employees/${employeeId}/payslips`,
    {
      lazy: true,
      server: true,
      immediate: false,
      baseURL: `${useRuntimeConfig().public.apiUrl}`,
      headers: {
        Authorization: `Bearer ${token.value}`,
      },
    }
  );

  async function fetchPayslips(refresh = false) {
    if (status.value !== "idle" && !refresh) return;
    await execute();
    payslips.value = data.value || [];
  }

  return {
    payslips,
    error,
    status,
    fetchPayslips,
  };
}
