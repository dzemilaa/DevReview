import { Link } from 'react-router-dom'
import { Card } from '@/components/common/Card'
import { ReviewRequestStatusBadge } from './ReviewRequestStatusBadge'
import type { ReviewRequest } from '@/types'
import { ROUTES } from '@/utils/constants'
import { formatDate, formatDifficulty } from '@/utils/format'

interface ReviewRequestCardProps {
  review: ReviewRequest
}

export function ReviewRequestCard({ review }: ReviewRequestCardProps) {
  return (
    <Link to={ROUTES.reviewDetail(review.id)} className="block transition hover:-translate-y-0.5">
      <Card className="h-full hover:border-brand-200 hover:shadow-md">
        <div className="flex items-start justify-between gap-3">
          <h3 className="text-lg font-semibold text-slate-900">{review.title}</h3>
          <ReviewRequestStatusBadge status={review.status} />
        </div>

        <p className="mt-2 line-clamp-2 text-sm text-slate-600">{review.description}</p>

        <div className="mt-4 flex flex-wrap gap-2">
          <span className="rounded-md bg-slate-100 px-2 py-1 text-xs font-medium text-slate-700">
            {review.programmingLanguage}
          </span>
          {review.framework && (
            <span className="rounded-md bg-slate-100 px-2 py-1 text-xs font-medium text-slate-700">
              {review.framework}
            </span>
          )}
          <span className="rounded-md bg-brand-50 px-2 py-1 text-xs font-medium text-brand-700">
            {formatDifficulty(review.difficulty)}
          </span>
          {review.isPaid && review.price > 0 ? (
            <span className="rounded-md bg-amber-50 px-2 py-1 text-xs font-medium text-amber-700">
              €{review.price.toFixed(2)}
            </span>
          ) : (
            <span className="rounded-md bg-emerald-50 px-2 py-1 text-xs font-medium text-emerald-700">
              Free
            </span>
          )}
        </div>

        {review.tags.length > 0 && (
          <div className="mt-3 flex flex-wrap gap-1.5">
            {review.tags.map((tag) => (
              <span key={tag} className="text-xs text-slate-500">
                #{tag}
              </span>
            ))}
          </div>
        )}

        <p className="mt-4 text-xs text-slate-400">{formatDate(review.createdAt)}</p>
      </Card>
    </Link>
  )
}
