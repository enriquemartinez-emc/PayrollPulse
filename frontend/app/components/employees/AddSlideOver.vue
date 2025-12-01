<script setup lang="ts">
import * as z from "zod";
import type { FormSubmitEvent } from "@nuxt/ui";

const { policies, fetchPolicies } = await usePolicies();
await fetchPolicies();

const { departments, status } = await useDepartments();
const { registerEmployee } = useEmployees();

const open = ref(false);

const schema = z.object({
  firstName: z
    .string("First name is required")
    .min(1, "Must be at least 1 character"),
  lastName: z
    .string("Last name is required")
    .min(1, "Must be at least 1 character"),
  email: z.email("Invalid email address"),
  baseSalary: z.number().positive("Base salary must be greater than zero"),
  departmentId: z.uuid("Select a valid department"),
  policies: z.array(z.string()).min(1, "Select a valid policy"),
});

type Schema = z.output<typeof schema>;

const state = reactive<Partial<Schema>>({
  firstName: undefined,
  lastName: undefined,
  email: undefined,
  baseSalary: 0,
  departmentId: undefined,
  policies: undefined,
});

const toast = useToast();

async function onSubmit(event: FormSubmitEvent<Schema>) {
  const newEmployee = await registerEmployee(event.data);

  if (newEmployee) {
    toast.add({
      title: "Success",
      description: "New employee has been registered.",
      color: "success",
    });
  }

  open.value = false;
}

function closeSlideover() {
  // logic to close slideover
  open.value = false;
}
</script>

<template>
  <USlideover
    v-model:open="open"
    title="New employee"
    description="Register a newly hired employee."
    :ui="{ footer: 'justify-end' }"
  >
    <UButton label="New employee" icon="i-lucide-plus" />

    <template #body>
      <UForm
        id="employee-form"
        :schema="schema"
        :state="state"
        class="gap-4 flex flex-col w-80"
        @submit="onSubmit"
      >
        <UFormField label="First Name" name="firstName">
          <UInput v-model="state.firstName" placeholder="John" />
        </UFormField>

        <UFormField label="Last Name" name="lastName">
          <UInput v-model="state.lastName" placeholder="Doe" />
        </UFormField>

        <UFormField label="Email" name="email">
          <UInput
            v-model="state.email"
            placeholder="john@company.com"
            type="email"
          />
        </UFormField>

        <UFormField label="Base Salary" name="baseSalary">
          <UInput
            v-model.number="state.baseSalary"
            type="number"
            min="0"
            placeholder="5000"
          />
        </UFormField>

        <UFormField label="Department" name="departmentId">
          <USelect
            v-model="state.departmentId"
            :items="
              departments?.map((department) => ({
                label: department.name,
                value: department.id,
              })) ?? []
            "
            :loading="status === 'pending'"
            placeholder="Select department"
          />
        </UFormField>

        <UFormField label="Policies" name="policies">
          <USelect
            v-model="state.policies"
            multiple
            :items="
              policies?.map((policy) => ({
                label: policy.name,
                value: policy.id,
              })) ?? []
            "
            class="w-48"
            placeholder="Select policies"
          />
        </UFormField>
      </UForm>
    </template>
    <template #footer>
      <UButton
        label="Cancel"
        color="primary"
        variant="outline"
        @click="closeSlideover"
      />
      <UButton
        label="Save"
        type="submit"
        color="primary"
        form="employee-form"
      />
    </template>
  </USlideover>
</template>
