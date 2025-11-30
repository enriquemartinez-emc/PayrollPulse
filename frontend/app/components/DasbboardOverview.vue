<script setup lang="ts">
import { subDays, formatISO } from "date-fns";

const selected = ref("30");

const period = computed(() => {
  if (selected.value === "30") {
    const end = new Date();
    const start = subDays(end, 30);
    return {
      start: formatISO(start, { representation: "date" }),
      end: formatISO(end, { representation: "date" }),
    };
  }
  if (selected.value === "90") {
    const end = new Date();
    const start = subDays(end, 90);
    return {
      start: formatISO(start, { representation: "date" }),
      end: formatISO(end, { representation: "date" }),
    };
  }
  return undefined;
});
</script>

<template>
  <div class="space-y-4">
    <div class="flex items-center justify-between">
      <h2 class="text-2xl font-semibold">Dashboard</h2>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-4">
      <div class="lg:col-span-2">
        <!-- <PayrollChart :period="period" /> -->
      </div>
      <div class="lg:col-span-1">
        <EmployeeChart />
      </div>
    </div>
  </div>
</template>
