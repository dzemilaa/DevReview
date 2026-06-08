import { Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { reviewRequestsApi } from '@/api'
import { Button } from '@/components/common/Button'
import { ReviewRequestList } from '@/components/reviewRequests/ReviewRequestList'
import { ROUTES } from '@/utils/constants'

export function MyReviewRequestsPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['review-requests', 'mine'],
    queryFn: reviewRequestsApi.getMine,
  })

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">My review requests</h1>
          <p className="mt-1 text-sm text-slate-500">Requests you have submitted</p>
        </div>
        <Link to={ROUTES.newReview}>
          <Button>New request</Button>
        </Link>
      </div>

      {error && (
        <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">
          Failed to load your review requests.
        </p>
      )}

      <ReviewRequestList
        reviews={data ?? []}
        isLoading={isLoading}
        emptyTitle="No requests yet"
        emptyDescription="Create a review request to get feedback from mentors."
        emptyAction={
          <Link to={ROUTES.newReview}>
            <Button>New request</Button>
          </Link>
        }
      />
    </div>
  )
}
