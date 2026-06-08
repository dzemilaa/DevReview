import {
  CommentType,
  DifficultyLevel,
  NotificationType,
  OfficeHourStatus,
  ReviewStatus,
} from '@/types'

const reviewStatusLabels: Record<ReviewStatus, string> = {
  [ReviewStatus.Open]: 'Open',
  [ReviewStatus.Claimed]: 'Claimed',
  [ReviewStatus.InReview]: 'In review',
  [ReviewStatus.Completed]: 'Completed',
  [ReviewStatus.Abandoned]: 'Abandoned',
  [ReviewStatus.PendingApproval]: 'Pending approval',
}

const difficultyLabels: Record<DifficultyLevel, string> = {
  [DifficultyLevel.Junior]: 'Junior',
  [DifficultyLevel.Mid]: 'Mid',
  [DifficultyLevel.Senior]: 'Senior',
}

const commentTypeLabels: Record<CommentType, string> = {
  [CommentType.General]: 'General',
  [CommentType.Suggestion]: 'Suggestion',
  [CommentType.Question]: 'Question',
  [CommentType.Praise]: 'Praise',
  [CommentType.Critical]: 'Critical',
}

export function formatReviewStatus(status: ReviewStatus): string {
  return reviewStatusLabels[status] ?? 'Unknown'
}

export function formatDifficulty(level: DifficultyLevel): string {
  return difficultyLabels[level] ?? 'Unknown'
}

export function formatCommentType(type: CommentType): string {
  return commentTypeLabels[type] ?? 'Unknown'
}

const LOCALE = 'en-US'

export function formatDate(value: string): string {
  return new Intl.DateTimeFormat(LOCALE, {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value))
}

export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat(LOCALE, {
    style: 'currency',
    currency: 'EUR',
  }).format(amount)
}

const officeHourStatusLabels: Record<OfficeHourStatus, string> = {
  [OfficeHourStatus.Available]: 'Available',
  [OfficeHourStatus.Booked]: 'Booked',
  [OfficeHourStatus.Completed]: 'Completed',
  [OfficeHourStatus.Cancelled]: 'Cancelled',
}

const notificationTypeLabels: Record<NotificationType, string> = {
  [NotificationType.NewComment]: 'New comment',
  [NotificationType.RequestClaimed]: 'Request claimed',
  [NotificationType.RequestAbandoned]: 'Request abandoned',
  [NotificationType.ReviewCompleted]: 'Review completed',
  [NotificationType.OfficeHourBooked]: 'Office hour booked',
  [NotificationType.OfficeHourCancelled]: 'Office hour cancelled',
  [NotificationType.OfficeHourCompleted]: 'Office hour completed',
  [NotificationType.NewRatingReceived]: 'New rating',
}

export function formatOfficeHourStatus(status: OfficeHourStatus): string {
  return officeHourStatusLabels[status] ?? 'Unknown'
}

export function formatNotificationType(type: NotificationType): string {
  return notificationTypeLabels[type] ?? 'Notification'
}
