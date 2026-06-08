import { create } from 'zustand'
import { authApi, profilesApi } from '@/api'
import type { User } from '@/types'
import { clearTokens, getAccessToken, setTokens } from '@/utils/storage'
import { queryClient } from '@/App'

interface AuthState {
  user: User | null
  isAuthenticated: boolean
  isLoading: boolean
  error: string | null
  login: (usernameOrEmail: string, password: string) => Promise<void>
  register: (payload: {
    userName: string
    email: string
    displayName: string
    password: string
    role?: string
  }) => Promise<void>
  logout: () => Promise<void>
  initialize: () => Promise<void>
  clearError: () => void
}

async function applyAuthResponse(
  response: Awaited<ReturnType<typeof authApi.login>>,
  set: (partial: Partial<AuthState>) => void,
) {
  setTokens(response.accessToken, response.refreshToken)
  set({
    user: response.user,
    isAuthenticated: true,
    error: null,
  })
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: false,
  isLoading: true,
  error: null,

  clearError: () => set({ error: null }),

  initialize: async () => {
    const token = getAccessToken()
    if (!token) {
      set({ isLoading: false, isAuthenticated: false, user: null })
      return
    }

    try {
      const profile = await profilesApi.getCurrent()
      set({
        user: profile,
        isAuthenticated: true,
        isLoading: false,
        error: null,
      })
    } catch {
      clearTokens()
      set({ isLoading: false, isAuthenticated: false, user: null })
    }
  },

  login: async (usernameOrEmail, password) => {
    set({ isLoading: true, error: null })
    try {
      const response = await authApi.login({ usernameOrEmail, password })
      queryClient.clear()
      await applyAuthResponse(response, set)
    } catch {
      set({ error: 'Invalid credentials. Please try again.' })
      throw new Error('Login failed')
    } finally {
      set({ isLoading: false })
    }
  },

  register: async (payload) => {
    set({ isLoading: true, error: null })
    try {
      const response = await authApi.register(payload)
      await applyAuthResponse(response, set)
    } catch {
      set({ error: 'Registration failed. Check your details and try again.' })
      throw new Error('Registration failed')
    } finally {
      set({ isLoading: false })
    }
  },

  logout: async () => {
    try {
      await authApi.logout()
    } catch {
      // ignore logout errors
    } finally {
      clearTokens()
      queryClient.clear()
      set({ user: null, isAuthenticated: false, error: null })
      window.location.href = '/'
    }
  },
}))
