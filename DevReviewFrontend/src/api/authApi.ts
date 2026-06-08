import { apiClient } from './client'
import type { AuthResponse, LoginRequest, RefreshTokenRequest, RegisterRequest } from '@/types'

export interface AuthMeResponse {
  userId: string
  isAuthenticated: boolean
}

export const authApi = {
  login: async (payload: LoginRequest): Promise<AuthResponse> => {
    const { data } = await apiClient.post<AuthResponse>('/auth/login', payload)
    return data
  },

  register: async (payload: RegisterRequest): Promise<AuthResponse> => {
    const { data } = await apiClient.post<AuthResponse>('/auth/register', payload)
    return data
  },

  refresh: async (payload: RefreshTokenRequest): Promise<AuthResponse> => {
    const { data } = await apiClient.post<AuthResponse>('/auth/refresh', payload)
    return data
  },

  logout: async (): Promise<void> => {
    await apiClient.post('/auth/logout')
  },

  getMe: async (): Promise<AuthMeResponse> => {
    const { data } = await apiClient.get<AuthMeResponse>('/auth/me')
    return data
  },
}
