export interface LanguageExpertise {
  language: string
  score: number
  totalReviews: number
  score1Count: number
  score2Count: number
  score3Count: number
  score4Count: number
  score5Count: number
}

export interface UserLanguage {
  language: string
  proficiency: number
}

export interface UserProfile {
  id: string
  userName: string
  email: string
  displayName: string
  bio?: string | null
  gitHubUrl?: string | null
  role: string
  yearsOfExperience: number
  hourlyRate?: number
  availableHoursPerWeek?: number
  averageMentorRating: number
  totalReviews: number
  languagesExpertise: LanguageExpertise[]
  userLanguages: UserLanguage[]
}

export interface UpdateProfilePayload {
  displayName: string
  bio?: string
  gitHubUrl?: string
  yearsOfExperience: number
  hourlyRate: number
  availableHoursPerWeek: number
  userLanguages: UserLanguage[]
}
