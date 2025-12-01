import type { Payroll } from "~/types";

export function usePayroll() {
  const payrolls = useState<Payroll[]>("payrolls", () => []);

  const { data, execute, status, page } = usePaginated<Payroll>("/payroll");

  async function fetchPayrolls(refresh = false) {
    if (status.value !== "idle" && !refresh) return;
    await execute();
    payrolls.value = data.value.items || [];
  }

  async function processPayroll(startDate: string, endDate: string) {
    try {
      const newPayroll = await $fetch<Payroll>("/payroll/process", {
        method: "POST",
        body: { startDate, endDate },
        baseURL: `${useRuntimeConfig().public.apiUrl}`,
        headers: {
          Authorization: `Bearer ${useCookie("auth-token").value}`,
        },
      });
      payrolls.value.unshift(newPayroll);
      return newPayroll;
    } catch (err: any) {
      console.log(err.message ?? "Failed to process a payroll");
      return null;
    }
  }

  return {
    payrolls,
    page,
    status,
    fetchPayrolls,
    processPayroll,
  };
}
