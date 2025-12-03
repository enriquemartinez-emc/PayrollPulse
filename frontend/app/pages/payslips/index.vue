<script setup lang="ts">
definePageMeta({
  middleware: ["auth"],
  roles: ["Employee"],
});

import { h, resolveComponent } from "vue";
import type { TableColumn } from "@nuxt/ui";
import type { Row } from "@tanstack/vue-table";
import type { Payslip } from "~/types";

const { payslips, fetchPayslips, status } = await usePayslips();
await fetchPayslips();

const UButton = resolveComponent("UButton");
const UDropdownMenu = resolveComponent("UDropdownMenu");

const columns: TableColumn<Payslip>[] = [
  {
    accessorKey: "start",
    header: "Start Date",
    cell: ({ row }) => {
      return new Date(`${row.getValue("start")}T00:00:00`).toLocaleString(
        "en-US",
        {
          day: "numeric",
          month: "short",
          year: "numeric",
          hour12: false,
        }
      );
    },
  },
  {
    accessorKey: "end",
    header: "End Date",
    cell: ({ row }) => {
      return new Date(`${row.getValue("end")}T00:00:00`).toLocaleString(
        "en-US",
        {
          day: "numeric",
          month: "short",
          year: "numeric",
          hour12: false,
        }
      );
    },
  },
  {
    accessorKey: "netPay",
    header: () => h("div", { class: "text-right" }, "Net Pay"),
    cell: ({ row }) => {
      const amount = Number.parseFloat(row.getValue("netPay"));

      const formatted = new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
      }).format(amount);

      return h("div", { class: "text-right font-medium" }, formatted);
    },
  },
  {
    accessorKey: "totalEarnings",
    header: () => h("div", { class: "text-right" }, "Total Earnings"),
    cell: ({ row }) => {
      const amount = Number.parseFloat(row.getValue("totalEarnings"));

      const formatted = new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
      }).format(amount);

      return h("div", { class: "text-right font-medium" }, formatted);
    },
  },
  {
    accessorKey: "totalDeductions",
    header: () => h("div", { class: "text-right" }, "Total Deductions"),
    cell: ({ row }) => {
      const amount = Number.parseFloat(row.getValue("totalDeductions"));

      const formatted = new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
      }).format(amount);

      return h("div", { class: "text-right font-medium" }, formatted);
    },
  },
  {
    id: "actions",
    cell: ({ row }) => {
      return h(
        "div",
        { class: "text-right" },
        h(
          UDropdownMenu,
          {
            content: {
              align: "end",
            },
            items: getRowItems(row),
            "aria-label": "Actions dropdown",
          },
          () =>
            h(UButton, {
              icon: "i-lucide-ellipsis-vertical",
              color: "neutral",
              variant: "ghost",
              class: "ml-auto",
              "aria-label": "Actions dropdown",
            })
        )
      );
    },
  },
];

function getRowItems(row: Row<Payslip>) {
  return [
    {
      type: "label",
      label: "Actions",
    },
    {
      label: "View payslip details",
      icon: "i-lucide-list",
      async onSelect() {
        await navigateTo(`/payslips/${row.original.id}`);
      },
    },
  ];
}
</script>

<template>
  <div class="flex-1 divide-y divide-accented w-full">
    <UTable
      :data="payslips"
      :columns="columns"
      :loading="status === 'pending'"
      class="flex-1 py-4"
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
