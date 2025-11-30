<script setup lang="ts">
import { VisDonut, VisSingleContainer, VisBulletLegend } from "@unovis/vue";

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

const legendItems = Object.entries(counts.value).map(([_, data]) => ({
  name: data.key.charAt(0).toUpperCase() + data.key.slice(1),
}));
</script>

<template>
  <UCard>
    <template #header>
      <div class="flex flex-col">
        <h3 class="text-lg font-semibold">Employees by Department</h3>
      </div>
    </template>

    <div class="flex flex-col">
      <VisBulletLegend :items="legendItems" />
      <VisSingleContainer :height="400">
        <VisDonut
          :data="counts"
          :value="(d:DataRecord) => d.value"
          :innerRadius="0.55"
          class="w-full"
        />
      </VisSingleContainer>
    </div>
  </UCard>
</template>
