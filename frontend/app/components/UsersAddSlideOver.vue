<script setup lang="ts">
import * as z from "zod";
import type { FormSubmitEvent } from "@nuxt/ui";

const { register } = await useUsers();
const { employees, status, query } = useEmployeeSearch();
const toast = useToast();
const open = ref(false);
const isEmployee = ref(false);

const schema = z.object({
  email: z.email("Invalid email address").optional(),
  password: z
    .string("Password is required")
    .min(8, "Must be at least 8 characters"),
  role: z.enum(["Admin", "Employee"], "Select a role"),
  employeeId: z.uuid("Select a valid employee"),
});

type Schema = z.output<typeof schema>;

const state = reactive<Partial<Schema>>({
  email: undefined,
  employeeId: undefined,
  password: undefined,
  role: undefined,
});

async function onSubmit(event: FormSubmitEvent<Schema>) {
  const newUser = await register(event.data);

  if (newUser) {
    toast.add({
      title: "Success",
      description: "New user has been registered.",
      color: "success",
    });
  }

  open.value = false;
}
</script>

<template>
  <USlideover
    v-model:open="open"
    title="New user"
    description="Register a new user."
    :ui="{ footer: 'justify-end' }"
  >
    <UButton label="New user" icon="i-lucide-plus" />

    <template #body>
      <UForm
        id="user-form"
        :schema="schema"
        :state="state"
        class="gap-4 flex flex-col w-80"
        @submit="onSubmit"
      >
        <div>
          <UCheckbox
            v-model="isEmployee"
            name="employee"
            label="Register user for an employee?"
            @update:model-value="state.employeeId = undefined"
          />
        </div>

        <UFormField v-if="isEmployee" label="Employee" name="employeeId">
          <UInputMenu
            v-model="state.employeeId"
            value-key="value"
            v-model:search-term="query"
            :items="employees"
            :loading="status === 'pending'"
            icon="i-lucide-user"
            placeholder="Select employee"
          />
        </UFormField>

        <UFormField v-if="!isEmployee" label="Email" name="email">
          <UInput
            v-model="state.email"
            placeholder="john@company.com"
            type="email"
          />
        </UFormField>

        <UFormField label="Password" name="password">
          <UInput v-model="state.password" type="password" />
        </UFormField>

        <UFormField label="Role" name="role">
          <USelect
            v-model="state.role"
            :items="[
              { label: 'Employee', value: 'Employee' },
              { label: 'Admin', value: 'Admin' },
            ]"
            placeholder="Select a role"
          />
        </UFormField>
      </UForm>
    </template>
    <template #footer>
      <UButton
        label="Cancel"
        color="primary"
        variant="outline"
        @click="open = false"
      />
      <UButton label="Save" type="submit" color="primary" form="user-form" />
    </template>
  </USlideover>
</template>
