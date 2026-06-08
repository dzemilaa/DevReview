import { apiClient } from './client'
import type {
  AbandonReviewPayload,
  AddCommentPayload,
  CodeFile,
  CodeFileInput,
  Comment,
  CreateReviewRequestPayload,
  FinalizeReviewPayload,
  ReviewRequest,
  ReviewRequestDetail,
} from '@/types'

export const reviewRequestsApi = {
  getAll: async (): Promise<ReviewRequest[]> => {
    const { data } = await apiClient.get<ReviewRequest[]>('/review-requests')
    return data
  },

  getMine: async (): Promise<ReviewRequest[]> => {
    const { data } = await apiClient.get<ReviewRequest[]>('/review-requests/my')
    return data
  },

  getClaimedByMe: async (): Promise<ReviewRequest[]> => {
    const { data } = await apiClient.get<ReviewRequest[]>('/review-requests/claimed-by-me')
    return data
  },

  getPublic: async (page = 1, pageSize = 20): Promise<ReviewRequest[]> => {
    const { data } = await apiClient.get<ReviewRequest[]>('/review-requests/public', {
      params: { page, pageSize },
    })
    return data
  },

  getById: async (id: string): Promise<ReviewRequestDetail> => {
    const { data } = await apiClient.get<ReviewRequestDetail>(`/review-requests/${id}`)
    return data
  },

  create: async (payload: CreateReviewRequestPayload): Promise<ReviewRequest> => {
    const { data } = await apiClient.post<ReviewRequest>('/review-requests', payload)
    return data
  },

  claim: async (id: string): Promise<ReviewRequest> => {
    const { data } = await apiClient.post<ReviewRequest>(`/review-requests/${id}/claim`)
    return data
  },

  abandon: async (id: string, payload: AbandonReviewPayload): Promise<ReviewRequest> => {
    const { data } = await apiClient.post<ReviewRequest>(`/review-requests/${id}/abandon`, payload)
    return data
  },

  addComment: async (id: string, payload: AddCommentPayload): Promise<Comment> => {
    const { data } = await apiClient.post<Comment>(`/review-requests/${id}/comments`, payload)
    return data
  },

  resolveComment: async (id: string, commentId: string): Promise<Comment> => {
    const { data } = await apiClient.post<Comment>(
      `/review-requests/${id}/comments/${commentId}/resolve`,
    )
    return data
  },

  parseZip: async (file: File): Promise<CodeFileInput[]> => {
    const form = new FormData()
    form.append('file', file)
    const { data } = await apiClient.post<CodeFileInput[]>('/review-requests/parse-zip', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    return data
  },

  submitPeerRating: async (id: string, rating: number): Promise<void> => {
    await apiClient.post(`/review-requests/${id}/rate`, { rating })
  },

  finalize: async (id: string, payload: FinalizeReviewPayload): Promise<ReviewRequest> => {
    const { data } = await apiClient.post<ReviewRequest>(`/review-requests/${id}/finalize`, payload)
    return data
  },

  applySuggestion: async (id: string, commentId: string): Promise<CodeFile> => {
    const { data } = await apiClient.post<CodeFile>(
      `/review-requests/${id}/comments/${commentId}/apply`,
    )
    return data
  },

  approveReview: async (id: string, approved: boolean): Promise<void> => {
    await apiClient.post(`/review-requests/${id}/approve`, approved, {
      headers: { 'Content-Type': 'application/json' },
    })
  },
}
