<script setup lang="ts">
const props = defineProps<{
  payslipId: string;
}>();

const { messages, status, explain } = useExplain();
const request = ref("");
const open = ref(false);

function onSubmit() {
  if (!request.value.trim()) return;
  explain(props.payslipId, request.value);
  request.value = "";
}
</script>

<template>
  <USlideover
    v-model:open="open"
    title="Payslip"
    description="Ask information about your payroll."
  >
    <UButton color="primary" icon="i-heroicons-chat-bubble-left-right">
      Ask information about this payslip
    </UButton>

    <template #body>
      <UChatPalette class="h-full">
        <UChatMessages
          :messages="messages"
          :status="status"
          :user="{
            side: 'right',
            variant: 'solid',
          }"
          :assistant="{ icon: 'i-lucide-bot' }"
        />
        <template #prompt>
          <UChatPrompt v-model="request" variant="soft" @submit="onSubmit">
            <UChatPromptSubmit :status="status" />
          </UChatPrompt>
        </template>
      </UChatPalette>
    </template>
  </USlideover>
</template>
