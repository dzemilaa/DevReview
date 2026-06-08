import type { DifficultyLevel, ReviewStatus } from './enums'

export interface SearchReviewRequestsParams {
  searchLanguage?: string
  framework?: string
  searchTag?: string
  searchDifficulty?: DifficultyLevel
  status?: ReviewStatus
  minPrice?: number
  maxPrice?: number
  page?: number
  pageSize?: number
}

export interface SearchMentorsParams {
  query?: string
  language?: string
  minimumRating?: number
  minimumYearsOfExperience?: number
  maxHourlyRate?: number
  page?: number
  pageSize?: number
}
