import type { Comment } from './comment'
import type { DifficultyLevel, ReviewStatus } from './enums'

export interface CodeFile {
  id: string
  fileName: string
  content: string
  orderIndex: number
}

export interface CodeFileInput {
  fileName: string
  content: string
  orderIndex: number
}

export interface ReviewRequest {
  id: string
  title: string
  description: string
  programmingLanguage: string
  framework: string
  difficulty: DifficultyLevel
  isPublic: boolean
  isPaid: boolean
  price: number
  tags: string[]
  status: ReviewStatus
  createdAt: string
  ownerId: string
  mentorId: string | null
}

export interface FinalReviewSummary {
  summary: string
  priorityFixes: string
  qualityScore: number
  createdAt: string
  authorRatingOfMentor: number | null
  mentorRatingOfAuthor: number | null
}

export interface ReviewRequestDetail extends ReviewRequest {
  claimedAt: string | null
  claimTimeout: string | null
  completedAt: string | null
  codeFiles: CodeFile[]
  comments: Comment[]
  finalReview: FinalReviewSummary | null
}

export interface CreateReviewRequestPayload {
  title: string
  description: string
  programmingLanguage: string
  framework: string
  difficulty: DifficultyLevel
  isPublic: boolean
  isPaid: boolean
  price: number
  tags: string[]
  codeFiles: CodeFileInput[]
}

export interface AbandonReviewPayload {
  reason: string
}

export interface FinalizeReviewPayload {
  summary: string
  priorityFixes: string
  qualityScore: number
}
