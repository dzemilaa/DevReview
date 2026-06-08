import type { LanguageExpertise } from './user'

export interface User {
  id: string
  userName: string
  email: string
  displayName: string
  role: string
  yearsOfExperience: number
  averageMentorRating: number
  totalReviews: number
  languagesExpertise: LanguageExpertise[]
}

export interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
  user: User
}

export interface LoginRequest {
  usernameOrEmail: string
  password: string
}

export interface RegisterRequest {
  userName: string
  email: string
  displayName: string
  password: string
  role?: string
}

export interface RefreshTokenRequest {
  refreshToken: string
}
