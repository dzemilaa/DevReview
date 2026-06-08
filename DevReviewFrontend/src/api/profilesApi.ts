import { apiClient } from './client'
import type { UpdateProfilePayload, UserProfile } from '@/types'

export const profilesApi = {
  getCurrent: async (): Promise<UserProfile> => {
    const { data } = await apiClient.get<UserProfile>('/profiles/me')
    return data
  },

  update: async (payload: UpdateProfilePayload): Promise<UserProfile> => {
    const { data } = await apiClient.put<UserProfile>('/profiles/me', payload)
    return data
  },
}
