import { apiClient } from './client'
import type {
  BookOfficeHourPayload,
  CancelOfficeHourPayload,
  CreateOfficeHourPayload,
  OfficeHour,
  OfficeHourDetail,
  SubmitOfficeHourFeedbackPayload,
} from '@/types'

export const officeHoursApi = {
  getAvailable: async (): Promise<OfficeHourDetail[]> => {
    const { data } = await apiClient.get<OfficeHourDetail[]>('/officehours/available')
    return data
  },

  getMentorSchedule: async (): Promise<OfficeHourDetail[]> => {
    const { data } = await apiClient.get<OfficeHourDetail[]>('/officehours/mentor/schedule')
    return data
  },

  getMyBookings: async (): Promise<OfficeHourDetail[]> => {
    const { data } = await apiClient.get<OfficeHourDetail[]>('/officehours/my-bookings')
    return data
  },


  create: async (payload: CreateOfficeHourPayload): Promise<OfficeHour> => {
    const { data } = await apiClient.post<OfficeHour>('/officehours', payload)
    return data
  },

  book: async (id: string, payload: BookOfficeHourPayload): Promise<OfficeHour> => {
    const { data } = await apiClient.post<OfficeHour>(`/officehours/${id}/book`, payload)
    return data
  },

  cancel: async (id: string, payload: CancelOfficeHourPayload): Promise<OfficeHour> => {
    const { data } = await apiClient.post<OfficeHour>(`/officehours/${id}/cancel`, payload)
    return data
  },

  submitFeedback: async (
    id: string,
    payload: SubmitOfficeHourFeedbackPayload,
  ): Promise<OfficeHourDetail> => {
    const { data } = await apiClient.post<OfficeHourDetail>(`/officehours/${id}/feedback`, payload)
    return data
  },
}
