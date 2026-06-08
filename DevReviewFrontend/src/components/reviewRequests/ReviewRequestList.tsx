import { ReviewRequestCard } from './ReviewRequestCard'
import { EmptyState } from '@/components/common/EmptyState'
import { Spinner } from '@/components/common/Spinner'
import type { ReviewRequest } from '@/types'

interface ReviewRequestListProps {
  reviews: ReviewRequest[]
  isLoading?: boolean
  emptyTitle?: string
  emptyDescription?: string
  emptyAction?: React.ReactNode
}

export function ReviewRequestList({
  reviews,
  isLoading = false,
  emptyTitle = 'No review requests',
  emptyDescription = 'There are no review requests to display yet.',
  emptyAction,
}: ReviewRequestListProps) {
  if (isLoading) {
    return <Spinner />
  }

  if (reviews.length === 0) {
    return (
      <EmptyState title={emptyTitle} description={emptyDescription} action={emptyAction} />
    )
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {reviews.map((review) => (
        <ReviewRequestCard key={review.id} review={review} />
      ))}
    </div>
  )
}
