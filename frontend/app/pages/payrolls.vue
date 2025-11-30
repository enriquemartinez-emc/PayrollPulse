<script setup lang="ts">
definePageMeta({ middleware: ["auth"], roles: ["Admin"] });

import type { PayPeriod } from "~/types";

const { payrolls, fetchPayrolls, processPayroll } = usePayroll();
await fetchPayrolls();

const toast = useToast();

const payPeriods = ref<PayPeriod[]>(getPayPeriodsFromOneYearBack());
const selectedPayPeriod = ref<string>(payPeriods.value[0]!.value);

async function handleGenerate() {
  const [start, end] = selectedPayPeriod.value.split("_to_");
  const newPayroll = await processPayroll(start!, end!);

  if (newPayroll) {
    toast.add({
      title: "Success",
      description: "New payroll has been generated.",
      color: "success",
    });
  }
}
</script>

<template>
  <div class="py-5 flex justify-end space-x-4">
    <USelect :items="payPeriods" v-model="selectedPayPeriod" />
    <UButton
      loading-auto
      icon="i-lucide-calculator"
      label="Generate Payroll"
      @click="handleGenerate"
    />
  </div>
  <div>
    <UTable
      :data="payrolls"
      class="flex-1"
      :ui="{
        base: 'table-fixed border-separate border-spacing-0',
        thead: '[&>tr]:bg-elevated/50 [&>tr]:after:content-none',
        tbody: '[&>tr]:last:[&>td]:border-b-0',
        th: 'first:rounded-l-lg last:rounded-r-lg border-y border-default first:border-l last:border-r',
        td: 'border-b border-default',
      }"
    />
  </div>
</template>
