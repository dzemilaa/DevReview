import { Badge } from '@/components/common/Badge'
import type { Notification } from '@/types'
import { formatDate, formatNotificationType } from '@/utils/format'
import { cn } from '@/utils/cn'

interface NotificationItemProps {
  notification: Notification
  onMarkRead?: (id: string) => void
  compact?: boolean
}

export function NotificationItem({
  notification,
  onMarkRead,
  compact = false,
}: NotificationItemProps) {
  return (
    <div
      className={cn(
        'flex gap-3 rounded-lg border p-3 transition-colors',
        notification.isRead
          ? 'border-slate-200 bg-white'
          : 'border-brand-200 bg-brand-50/50',
        compact && 'p-2.5',
      )}
    >
      <div className="min-w-0 flex-1">
        <div className="flex flex-wrap items-center gap-2">
          <Badge variant={notification.isRead ? 'neutral' : 'info'}>
            {formatNotificationType(notification.type)}
          </Badge>
          <span className="text-xs text-slate-400">{formatDate(notification.createdAt)}</span>
        </div>
        <p className={cn('mt-1 text-sm text-slate-700', compact && 'line-clamp-2')}>
          {notification.message}
        </p>
      </div>
      {!notification.isRead && onMarkRead && (
        <button
          type="button"
          onClick={() => onMarkRead(notification.id)}
          className="shrink-0 text-xs font-medium text-brand-600 hover:text-brand-700"
        >
          Mark read
        </button>
      )}
    </div>
  )
}
