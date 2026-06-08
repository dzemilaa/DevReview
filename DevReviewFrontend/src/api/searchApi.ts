import { apiClient } from './client'
import type { MentorRanking, PagedResult, ReviewRequest, SearchMentorsParams, SearchReviewRequestsParams } from '@/types'

export const searchApi = {
  searchReviewRequests: async (
    params: SearchReviewRequestsParams = {},
  ): Promise<PagedResult<ReviewRequest>> => {
    const { data } = await apiClient.get<PagedResult<ReviewRequest>>('/search/review-requests', {
      params,
    })
    return data
  },

  searchMentors: async (params: SearchMentorsParams = {}): Promise<PagedResult<MentorRanking>> => {
    const { data } = await apiClient.get<PagedResult<MentorRanking>>('/search/mentors', { params })
    return data
  },
}
