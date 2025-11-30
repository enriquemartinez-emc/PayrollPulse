<script setup>
definePageMeta({
  layout: false,
});

const goHome = async () => await navigateTo("/");
</script>

<template>
  <UContainer class="py-20 text-center">
    <UCard>
      <h1 class="text-2xl font-bold mb-4">Unauthorized</h1>
      <p class="mb-6">You don’t have permission to access this page.</p>
      <UButton color="primary" @click="goHome"> Go back to Home </UButton>
    </UCard>
  </UContainer>
</template>
