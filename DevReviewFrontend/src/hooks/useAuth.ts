import { useAuthStore } from '@/store/authStore'

export function useAuth() {
  const user = useAuthStore((s) => s.user)
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)
  const isLoading = useAuthStore((s) => s.isLoading)
  const error = useAuthStore((s) => s.error)
  const login = useAuthStore((s) => s.login)
  const register = useAuthStore((s) => s.register)
  const logout = useAuthStore((s) => s.logout)
  const clearError = useAuthStore((s) => s.clearError)

  const isAdmin = user?.role?.toLowerCase() === 'admin'
  const isMentor = user?.role?.toLowerCase() === 'mentor' || isAdmin

  return {
    user,
    isAuthenticated,
    isLoading,
    error,
    isMentor,
    isAdmin,
    login,
    register,
    logout,
    clearError,
  }
}
