<script setup lang="ts">
definePageMeta({ middleware: ["auth"], roles: ["Admin"] });
</script>

<template>
  <div class="page-dashboard">
    <DashboardOverview />
  </div>
</template>

<style scoped>
.page-dashboard {
  padding: 1.25rem;
}
</style>
