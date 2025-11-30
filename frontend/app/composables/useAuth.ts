import type { LoggedUser, User } from "~/types";

export function useAuth() {
  const user = useState<User | null | undefined>("auth-user", () => null);
  const toast = useToast();

  const token = useCookie("auth-token", {
    httpOnly: false, // Allow client access for API headers
    secure: import.meta.env.PROD,
    sameSite: "strict",
    maxAge: 60 * 10, // 10 minutes
  });

  const { data: userData, refresh: refreshUser } = useFetch<User | null>(
    "/auth/me",
    {
      baseURL: useRuntimeConfig().public.apiUrl,
      headers: { Authorization: `Bearer ${token.value}` },
      immediate: false,
      lazy: true,
      server: false,
    }
  );

  const isAuthenticated = computed(() => !!token.value);
  const isAdmin = computed(() => user.value?.role === "Admin");
  const isEmployee = computed(() => user.value?.role === "Employee");

  async function login(email: string, password: string) {
    try {
      const response = await $fetch<LoggedUser>("/auth/login", {
        method: "POST",
        body: { email, password },
        baseURL: useRuntimeConfig().public.apiUrl,
        onResponseError({ response }) {
          if (response.status === 401) {
            toast.add({
              title: "Invalid credentials",
              description: "Please check your email and password.",
              icon: "i-heroicons-exclamation-triangle",
            });
          }
        },
      });

      if (response) {
        token.value = response.token;
        user.value = {
          id: response.userId,
          email: response.email,
          role: response.role,
          employeeId: response.employeeId,
        };
      }

      return { user: user.value };
    } catch (err: any) {
      return { ok: false, error: err?.data ?? err?.message ?? "Login failed" };
    }
  }

  async function logout() {
    token.value = null;
    user.value = null;
    await navigateTo("/login");
  }

  async function checkAuth() {
    if (token.value && !user.value) {
      await refreshUser();
      user.value = userData.value;
    }
  }

  return {
    user,
    token,
    isAuthenticated,
    isAdmin,
    isEmployee,
    checkAuth,
    login,
    logout,
  };
}
