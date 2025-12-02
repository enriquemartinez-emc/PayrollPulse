<script setup lang="ts">
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  BarController,
} from "chart.js";
import { Chart } from "vue-chartjs";

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  BarController
);

const { payrolls, fetchPayrolls } = usePayroll();
const rows = ref<{ label: string; gross: number; net: number }[]>([]);

await fetchPayrolls();
update();

watch(() => payrolls.value, update);

function update() {
  const list = payrolls.value || [];
  rows.value = list.map((p) => ({
    label: p.startDate,
    gross: p.totalGross,
    net: p.totalNet,
  }));
}

const grossColor = "#4F46E5";
const netColor = "#10B981";

const chartData = computed(() => ({
  labels: rows.value.map((r) => r.label),
  datasets: [
    {
      label: "Gross",
      data: rows.value.map((r) => r.gross),
      backgroundColor: grossColor,
    },
    {
      label: "Net",
      data: rows.value.map((r) => r.net),
      backgroundColor: netColor,
    },
  ],
}));

const chartOptions = { responsive: true };
</script>

<template>
  <UCard>
    <template #header>
      <h3 class="text-lg font-semibold">Payroll Totals</h3>
      <p class="text-sm text-gray-500">
        Gross vs Net across recent payroll runs
      </p>
    </template>

    <div class="w-full h-72">
      <Chart type="bar" :data="chartData" :options="chartOptions" />
    </div>
  </UCard>
</template>
