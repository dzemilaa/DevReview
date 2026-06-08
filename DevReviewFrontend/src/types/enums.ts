export const ReviewStatus = {
  Open: 1,
  Claimed: 2,
  InReview: 3,
  Completed: 4,
  Abandoned: 5,
  PendingApproval: 6,
} as const

export type ReviewStatus = (typeof ReviewStatus)[keyof typeof ReviewStatus]

export const DifficultyLevel = {
  Junior: 0,
  Mid: 1,
  Senior: 2,
} as const

export type DifficultyLevel = (typeof DifficultyLevel)[keyof typeof DifficultyLevel]

export const CommentType = {
  General: 0,
  Suggestion: 1,
  Question: 2,
  Praise: 3,
  Critical: 4,
} as const

export type CommentType = (typeof CommentType)[keyof typeof CommentType]

export const OfficeHourStatus = {
  Available: 0,
  Booked: 1,
  Completed: 2,
  Cancelled: 3,
} as const

export type OfficeHourStatus = (typeof OfficeHourStatus)[keyof typeof OfficeHourStatus]

export const NotificationType = {
  NewComment: 0,
  RequestClaimed: 1,
  RequestAbandoned: 2,
  ReviewCompleted: 3,
  OfficeHourBooked: 4,
  OfficeHourCancelled: 5,
  OfficeHourCompleted: 6,
  NewRatingReceived: 7,
} as const

export type NotificationType = (typeof NotificationType)[keyof typeof NotificationType]
