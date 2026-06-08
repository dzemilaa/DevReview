import { apiClient } from './client'
import type { CreateTagPayload, Tag, UpdateTagPayload } from '@/types'

export const tagsApi = {
  getAll: async (): Promise<Tag[]> => {
    const { data } = await apiClient.get<Tag[]>('/tags')
    return data
  },

  create: async (payload: CreateTagPayload): Promise<Tag> => {
    const { data } = await apiClient.post<Tag>('/tags', payload)
    return data
  },

  update: async (id: string, payload: UpdateTagPayload): Promise<Tag> => {
    const { data } = await apiClient.put<Tag>(`/tags/${id}`, payload)
    return data
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/tags/${id}`)
  },
}
