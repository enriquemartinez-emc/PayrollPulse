export default defineNuxtRouteMiddleware(async (to) => {
  const { user, isAuthenticated, checkAuth } = useAuth();
  await checkAuth();

  if (!isAuthenticated.value) {
    return navigateTo({ path: "/login", query: { redirect: to.fullPath } });
  }

  const requiredRoles = to.meta.roles as string[] | undefined;
  if (!requiredRoles || requiredRoles.length === 0) return;

  if (!user.value || !requiredRoles.includes(user.value.role)) {
    return navigateTo("/unauthorized");
  }
});
