import { Badge } from '@/components/common/Badge'
import type { ReviewStatus } from '@/types'
import { ReviewStatus as ReviewStatusEnum } from '@/types'
import { formatReviewStatus } from '@/utils/format'

interface ReviewRequestStatusBadgeProps {
  status: ReviewStatus
}

function getVariant(status: ReviewStatus): 'default' | 'success' | 'warning' | 'info' | 'neutral' {
  switch (status) {
    case ReviewStatusEnum.Open:
      return 'info'
    case ReviewStatusEnum.Claimed:
    case ReviewStatusEnum.InReview:
      return 'warning'
    case ReviewStatusEnum.Completed:
      return 'success'
    case ReviewStatusEnum.Abandoned:
      return 'neutral'
    case ReviewStatusEnum.PendingApproval:
      return 'warning'
    default:
      return 'default'
  }
}

export function ReviewRequestStatusBadge({ status }: ReviewRequestStatusBadgeProps) {
  return <Badge variant={getVariant(status)}>{formatReviewStatus(status)}</Badge>
}
