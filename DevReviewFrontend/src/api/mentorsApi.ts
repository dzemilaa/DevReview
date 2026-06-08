import { apiClient } from './client'
import type { MentorPublicProfile, MentorRanking, TopMentorsParams } from '@/types'

export const mentorsApi = {
  getProfile: async (mentorId: string): Promise<MentorPublicProfile> => {
    const { data } = await apiClient.get<MentorPublicProfile>(`/mentors/${mentorId}`)
    return data
  },

  getTop: async (params: TopMentorsParams = {}): Promise<MentorRanking[]> => {
    const { data } = await apiClient.get<MentorRanking[]>('/mentors/top', { params })
    return data
  },

  getFollowing: async (): Promise<string[]> => {
    const { data } = await apiClient.get<string[]>('/mentors/following')
    return data
  },

  getFollowingDetails: async (): Promise<MentorRanking[]> => {
    const { data } = await apiClient.get<MentorRanking[]>('/mentors/following/details')
    return data
  },

  follow: async (mentorId: string): Promise<void> => {
    await apiClient.post(`/mentors/${mentorId}/follow`)
  },

  unfollow: async (mentorId: string): Promise<void> => {
    await apiClient.delete(`/mentors/${mentorId}/follow`)
  },
}
