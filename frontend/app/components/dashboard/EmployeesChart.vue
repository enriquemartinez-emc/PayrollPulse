<script setup lang="ts">
import { Chart as ChartJS, Title, Tooltip, Legend, ArcElement } from "chart.js";
import { Chart } from "vue-chartjs";

ChartJS.register(Title, Tooltip, Legend, ArcElement);

const { employees, fetchEmployees } = useEmployees();

type DataRecord = { key: string; value: number };
const counts = ref<DataRecord[]>([]);

await fetchEmployees();
updateCounts();

watch(() => employees.value, updateCounts);

function updateCounts() {
  const listOfEmployees = employees.value || [];
  const map = new Map<string, number>();

  for (const employee of listOfEmployees) {
    const department = employee.department ?? "Unassigned";
    map.set(department, (map.get(department) ?? 0) + 1);
  }

  counts.value = [...map.entries()].map(([key, value]) => ({ key, value }));
}

// Chart.js data
const chartData = computed(() => ({
  labels: counts.value.map((d) => d.key),
  datasets: [
    {
      label: "Employees",
      data: counts.value.map((d) => d.value),
      backgroundColor: [
        "#4F46E5", // indigo
        "#10B981", // green
        "#F59E0B", // amber
        "#EF4444", // red
        "#6366F1", // violet
        "#14B8A6", // teal
      ],
    },
  ],
}));

const chartOptions = {
  responsive: true,
  plugins: {
    legend: {
      position: "right",
    },
    title: {
      display: false,
    },
  },
};
</script>

<template>
  <UCard>
    <template #header>
      <div class="flex flex-col">
        <h3 class="text-lg font-semibold">Employees</h3>
        <p class="text-sm text-gray-500">Employees by Department</p>
      </div>
    </template>

    <div class="w-full h-96">
      <!-- Chart.js doughnut chart -->
      <Chart type="doughnut" :data="chartData" :options="chartOptions" />
    </div>
  </UCard>
</template>
