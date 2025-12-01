<script setup lang="ts">
import type { Payslip } from "~/types";

const route = useRoute();
const id = route.params.id as string;

const { token } = useAuth();

const { data } = await useFetch<Payslip>(`/payslips/${id}`, {
  default: () => ({
    id: "",
    start: "",
    end: "",
    netPay: 0,
    totalBonuses: 0,
    totalDeductions: 0,
    items: [],
  }),
  lazy: true,
  server: false,
  baseURL: `${useRuntimeConfig().public.apiUrl}`,
  headers: {
    Authorization: `Bearer ${token.value}`,
  },
});

// Currency formatter for US dollars
const usd = new Intl.NumberFormat("en-US", {
  style: "currency",
  currency: "USD",
});

const formattedTotals = computed(() => ({
  totalBonuses: usd.format(data.value.totalBonuses ?? 0),
  totalDeductions: usd.format(data.value.totalDeductions ?? 0),
  netPay: usd.format(data.value.netPay ?? 0),
}));
</script>

<template>
  <div class="py-5 flex justify-end">
    <ExplainSlideOver :payslip-id="id" />
  </div>
  <div class="py-2">
    <UCard class="p-4">
      <div class="flex items-center justify-between gap-4">
        <div>
          <div>Period</div>
          <div>{{ data.start }} — {{ data.end }}</div>
        </div>

        <div class="text-right">
          <div>Total Bonuses</div>
          <div>
            {{ formattedTotals.totalBonuses }}
          </div>
        </div>

        <div class="text-right">
          <div>Total Deductions</div>
          <div>
            {{ formattedTotals.totalDeductions }}
          </div>
        </div>

        <div class="text-right">
          <div>Net Pay</div>
          <div class="text-primary">
            {{ formattedTotals.netPay }}
          </div>
        </div>
      </div>

      <hr class="my-4 border-gray-200" />

      <ul class="space-y-2">
        <li v-for="(item, index) in data.items" :key="index" class="py-2">
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-2 items-center">
            <div class="sm:col-span-2">
              <div>
                {{ item.compensationName }}
              </div>
              <div>{{ item.compensationType }} • {{ item.policyName }}</div>
            </div>
            <div class="text-right">
              {{ usd.format(item.amount) }}
            </div>
          </div>
        </li>
      </ul>
    </UCard>
  </div>
</template>
