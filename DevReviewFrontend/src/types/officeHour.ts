import type { OfficeHourStatus } from './enums'

export interface OfficeHour {
  id: string
  mentorId: string
  startTime: string
  durationMinutes: number
  endTime: string
  topic: string
  price: number | null
  status: OfficeHourStatus
  bookedById: string | null
  bookingDescription: string | null
}

export interface OfficeHourDetail extends OfficeHour {
  mentorName: string
  mentorEmail: string
  bookedByName: string | null
  bookedByEmail: string | null
  mentorImpression: string | null
  authorImpression: string | null
}

export interface CreateOfficeHourPayload {
  startTime: string
  durationMinutes: number
  topic: string
  price?: number | null
}

export interface BookOfficeHourPayload {
  bookingDescription?: string
}

export interface CancelOfficeHourPayload {
  cancellationReason?: string
}

export interface SubmitOfficeHourFeedbackPayload {
  impression: string
}
