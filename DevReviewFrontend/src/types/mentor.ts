import type { LanguageExpertise } from './user'
import type { ReviewRequest } from './reviewRequest'

export interface MentorRanking {
  mentorId: string
  displayName: string
  userName: string
  bio?: string
  overallRating: number
  totalReviews: number
  yearsOfExperience: number
  hourlyRate?: number
  availableHoursPerWeek?: number
  profileLanguages: string[]
  languagesExpertise: LanguageExpertise[]
}

export interface MentorPublicProfile {
  id: string
  displayName: string
  userName: string
  bio?: string
  gitHubUrl?: string
  yearsOfExperience: number
  hourlyRate: number
  availableHoursPerWeek: number
  averageMentorRating: number
  totalReviews: number
  profileLanguages: string[]
  languagesExpertise: LanguageExpertise[]
  publicReviews: ReviewRequest[]
}

export interface TopMentorsParams {
  language?: string
  minimumRating?: number
  minimumYearsOfExperience?: number
  period?: 'week'
}
