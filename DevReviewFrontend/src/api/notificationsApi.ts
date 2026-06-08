import { apiClient } from './client'
import type { Notification } from '@/types'

export const notificationsApi = {
  getAll: async (onlyUnread = false): Promise<Notification[]> => {
    const { data } = await apiClient.get<Notification[]>('/notifications', {
      params: { onlyUnread },
    })
    return data
  },

  markRead: async (notificationId: string): Promise<void> => {
    await apiClient.post(`/notifications/${notificationId}/read`)
  },

  markAllRead: async (): Promise<void> => {
    await apiClient.post('/notifications/read-all')
  },
}
