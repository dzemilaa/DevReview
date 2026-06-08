import { Card } from '@/components/common/Card'
import type { FinalReviewSummary } from '@/types'
import { formatDate } from '@/utils/format'

interface FinalReviewSummaryCardProps {
  review: FinalReviewSummary
}

export function FinalReviewSummaryCard({ review }: FinalReviewSummaryCardProps) {
  return (
    <Card className="border-emerald-200 bg-emerald-50/30">
      <h3 className="text-lg font-semibold text-slate-900">Final review summary</h3>
      <div className="mt-3 flex items-center gap-3">
        <span className="text-sm text-slate-500">Quality score</span>
        <span className="text-2xl font-bold text-slate-900">{review.qualityScore}/5</span>
      </div>
      <div className="mt-4 space-y-3 text-sm">
        <div>
          <p className="font-medium text-slate-700">Summary</p>
          <p className="mt-1 whitespace-pre-wrap text-slate-600">{review.summary}</p>
        </div>
        <div>
          <p className="font-medium text-slate-700">Priority fixes</p>
          <p className="mt-1 whitespace-pre-wrap text-slate-600">{review.priorityFixes}</p>
        </div>
      </div>
      <p className="mt-3 text-xs text-slate-400">Submitted {formatDate(review.createdAt)}</p>
    </Card>
  )
}
