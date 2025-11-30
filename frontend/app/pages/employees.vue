<script setup lang="ts">
definePageMeta({ middleware: ["auth"], roles: ["Admin"] });

const { employees, fetchEmployees } = useEmployees();
await fetchEmployees();
</script>

<template>
  <div class="py-5 flex justify-end">
    <EmployeesAddSlideOver />
  </div>
  <div>
    <div class="flex-1 divide-y divide-accented w-full">
      <UTable
        :data="employees"
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
  </div>
</template>
