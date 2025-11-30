import type { User, UserToSubmit } from "~/types";

export async function useUsers() {
  const users = useState<User[]>("users", () => []);
  const { token } = useAuth();
  const toast = useToast();

  const { data, status, execute } = await useFetch<User[]>("/auth/users", {
    baseURL: useRuntimeConfig().public.apiUrl,
    headers: {
      Authorization: `Bearer ${token.value}`,
    },
    immediate: false,
  });

  async function fetchUsers(refresh = false) {
    if (status.value !== "idle" && !refresh) return;
    await execute();
    users.value = data.value || [];
  }

  async function register(userToSubmit: UserToSubmit) {
    const newUser = await $fetch<User>("/auth/register", {
      method: "POST",
      body: {
        email: userToSubmit.email || null,
        password: userToSubmit.password,
        role: userToSubmit.role,
        employeeId: userToSubmit.employeeId || null,
      },
      baseURL: useRuntimeConfig().public.apiUrl,
      headers: {
        Authorization: `Bearer ${token.value}`,
      },
    });

    if (newUser) {
      users.value.unshift(newUser);
      toast.add({
        title: "Success",
        description: "User created successfully.",
        icon: "i-lucide-thumbs-up",
      });

      return newUser;
    }
  }

  return { users, status, register, fetchUsers };
}
