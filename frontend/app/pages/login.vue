<script setup lang="ts">
definePageMeta({
  layout: false,
});

import * as z from "zod";
import type { FormSubmitEvent, AuthFormField } from "@nuxt/ui";

const { login } = useAuth();
const toast = useToast();

const fields = ref<AuthFormField[]>([
  {
    name: "email",
    type: "email",
    label: "Email",
    placeholder: "Enter your email",
    required: true,
  },
  {
    name: "password",
    label: "Password",
    type: "password",
    placeholder: "Enter your password",
    required: true,
  },
]);

const schema = z.object({
  email: z.email("Invalid email"),
  password: z
    .string("Password is required")
    .min(8, "Must be at least 8 characters"),
});

type Schema = z.output<typeof schema>;

async function onSubmit(payload: FormSubmitEvent<Schema>) {
  const { email, password } = payload.data;

  try {
    const { user } = await login(email, password);

    if (user?.role === "Admin") {
      await navigateTo("/dashboard");
    } else {
      await navigateTo(`/payslips`);
    }
  } catch (error) {
    toast.add({ title: "Error", description: "Unauthorized" });
  }
}
</script>

<template>
  <div class="flex flex-col items-center justify-center gap-4 mt-10">
    <UPageCard class="w-full max-w-md">
      <UAuthForm
        :schema="schema"
        :fields="fields"
        title="Login"
        icon="i-lucide-lock"
        @submit="onSubmit"
      />
    </UPageCard>
  </div>
</template>
