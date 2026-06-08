import { useNavigate } from 'react-router-dom'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { reviewRequestsApi } from '@/api'
import { ReviewRequestForm } from '@/components/reviewRequests/ReviewRequestForm'
import type { CreateReviewRequestPayload } from '@/types'
import { ROUTES } from '@/utils/constants'

export function NewReviewRequestPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const mutation = useMutation({
    mutationFn: (payload: CreateReviewRequestPayload) => reviewRequestsApi.create(payload),
    onSuccess: (data) => {
      void queryClient.invalidateQueries({ queryKey: ['review-requests'] })
      void navigate(ROUTES.reviewDetail(data.id))
    },
  })

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">New review request</h1>
        <p className="mt-1 text-sm text-slate-500">
          Describe your code and what kind of feedback you need
        </p>
      </div>

      {mutation.isError && (
        <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">
          Could not create the review request. Please check your input and try again.
        </p>
      )}

      <ReviewRequestForm
        onSubmit={async (payload) => {
          await mutation.mutateAsync(payload)
        }}
        isSubmitting={mutation.isPending}
      />
    </div>
  )
}
