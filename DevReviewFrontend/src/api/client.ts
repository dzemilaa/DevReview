import axios from 'axios'
import type { AuthResponse } from '@/types'
import { clearTokens, getAccessToken, getRefreshToken, setTokens } from '@/utils/storage'

const baseURL = import.meta.env.VITE_API_BASE_URL ?? '/api'

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

let refreshPromise: Promise<string> | null = null

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    if (error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error)
    }

    const refreshToken = getRefreshToken()
    if (!refreshToken) {
      clearTokens()
      return Promise.reject(error)
    }

    originalRequest._retry = true

    if (!refreshPromise) {
      refreshPromise = axios
        .post<AuthResponse>(`${baseURL}/auth/refresh`, { refreshToken })
        .then(({ data }) => {
          setTokens(data.accessToken, data.refreshToken)
          return data.accessToken
        })
        .finally(() => {
          refreshPromise = null
        })
    }

    try {
      const accessToken = await refreshPromise
      originalRequest.headers.Authorization = `Bearer ${accessToken}`
      return apiClient(originalRequest)
    } catch {
      clearTokens()
      return Promise.reject(error)
    }
  },
)
