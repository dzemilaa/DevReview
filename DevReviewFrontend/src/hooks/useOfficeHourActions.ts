import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { officeHoursApi } from '@/api'

export function useOfficeHourActions() {
  const queryClient = useQueryClient()
  const [impressions, setImpressions] = useState<Record<string, string>>({})

  const cancelMutation = useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string }) =>
      officeHoursApi.cancel(id, { cancellationReason: reason || undefined }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['office-hours'] })
    },
  })

  const feedbackMutation = useMutation({
    mutationFn: ({ id, impression }: { id: string; impression: string }) =>
      officeHoursApi.submitFeedback(id, { impression }),
    onSuccess: (detail) => {
      const imp = detail.authorImpression ?? detail.mentorImpression
      if (imp) {
        setImpressions((prev) => ({ ...prev, [detail.id]: imp }))
      }
      void queryClient.invalidateQueries({ queryKey: ['office-hours'] })
      void queryClient.invalidateQueries({ queryKey: ['notifications'] })
    },
  })

  return {
    impressions,
    cancelMutation,
    feedbackMutation,
    isActionLoading: cancelMutation.isPending || feedbackMutation.isPending,
    onCancel: async (id: string, reason: string) => {
      await cancelMutation.mutateAsync({ id, reason })
    },
    onFeedback: async (id: string, impression: string) => {
      await feedbackMutation.mutateAsync({ id, impression })
    },
  }
}
