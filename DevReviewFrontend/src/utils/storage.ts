import { STORAGE_KEYS } from './constants'

export function getAccessToken(): string | null {
  return sessionStorage.getItem(STORAGE_KEYS.accessToken)
}

export function getRefreshToken(): string | null {
  return sessionStorage.getItem(STORAGE_KEYS.refreshToken)
}

export function setTokens(accessToken: string, refreshToken: string): void {
  sessionStorage.setItem(STORAGE_KEYS.accessToken, accessToken)
  sessionStorage.setItem(STORAGE_KEYS.refreshToken, refreshToken)
}

export function clearTokens(): void {
  sessionStorage.removeItem(STORAGE_KEYS.accessToken)
  sessionStorage.removeItem(STORAGE_KEYS.refreshToken)
}
