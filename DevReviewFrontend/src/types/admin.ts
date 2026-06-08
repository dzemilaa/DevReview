export interface AdminUser {
  id: string
  userName: string
  email: string
  displayName: string
  role: string
  isActive: boolean
  isBlocked: boolean
  blockReason: string | null
  blockedAt: string | null
  yearsOfExperience: number
  hourlyRate: number
  averageMentorRating: number
  totalReviews: number
}

export interface AdminStatistics {
  totalUsers: number
  totalMentors: number
  totalAuthors: number
  totalReviewRequests: number
  totalCompletedReviews: number
  totalOfficeHourBookings: number
  totalActiveUsers: number
}

export interface BlockUserPayload {
  reason: string
}
