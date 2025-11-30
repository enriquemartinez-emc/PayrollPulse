<script setup lang="ts">
import type { NavigationMenuItem } from "@nuxt/ui";

const { isAdmin, logout } = useAuth();

const items = ref<NavigationMenuItem[]>([
  ...(isAdmin.value
    ? [
        {
          label: "Dashboard",
          to: "/dashboard",
        },
        {
          label: "Employees",
          to: "/employees",
        },
        {
          label: "Payroll",
          to: "/payrolls",
        },
        {
          label: "Users",
          to: "/users",
        },
      ]
    : [
        {
          label: "Payslips",
          to: `/payslips`,
        },
      ]),
]);
</script>

<template>
  <UHeader>
    <template #title> Payroll </template>

    <UNavigationMenu highlight :items="items" />

    <template #right>
      <UColorModeButton />

      <UTooltip text="Logout">
        <UButton
          color="neutral"
          variant="ghost"
          @click="logout"
          icon="i-lucide-log-out"
        />
      </UTooltip>
    </template>
  </UHeader>
</template>
