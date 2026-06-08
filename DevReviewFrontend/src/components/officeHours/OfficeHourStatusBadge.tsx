import { Badge } from '@/components/common/Badge'
import type { OfficeHourStatus } from '@/types'
import { OfficeHourStatus as OfficeHourStatusEnum } from '@/types'
import { formatOfficeHourStatus } from '@/utils/format'

interface OfficeHourStatusBadgeProps {
  status: OfficeHourStatus
}

function getVariant(status: OfficeHourStatus): 'default' | 'success' | 'warning' | 'info' | 'neutral' {
  switch (status) {
    case OfficeHourStatusEnum.Available:
      return 'info'
    case OfficeHourStatusEnum.Booked:
      return 'warning'
    case OfficeHourStatusEnum.Completed:
      return 'success'
    case OfficeHourStatusEnum.Cancelled:
      return 'neutral'
    default:
      return 'default'
  }
}

export function OfficeHourStatusBadge({ status }: OfficeHourStatusBadgeProps) {
  return <Badge variant={getVariant(status)}>{formatOfficeHourStatus(status)}</Badge>
}
