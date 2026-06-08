import { apiClient } from './client'
import type { AdminStatistics, AdminUser, BlockUserPayload } from '@/types'

export const adminApi = {
  getUsers: async (): Promise<AdminUser[]> => {
    const { data } = await apiClient.get<AdminUser[]>('/admin/users')
    return data
  },

  getStatistics: async (): Promise<AdminStatistics> => {
    const { data } = await apiClient.get<AdminStatistics>('/admin/statistics')
    return data
  },

  blockUser: async (userId: string, payload: BlockUserPayload): Promise<void> => {
    await apiClient.post(`/admin/users/${userId}/block`, payload)
  },

  unblockUser: async (userId: string): Promise<void> => {
    await apiClient.post(`/admin/users/${userId}/unblock`)
  },

  deleteReviewRequest: async (reviewRequestId: string): Promise<void> => {
    await apiClient.delete(`/admin/review-requests/${reviewRequestId}`)
  },

  deleteComment: async (commentId: string): Promise<void> => {
    await apiClient.delete(`/admin/comments/${commentId}`)
  },
}
