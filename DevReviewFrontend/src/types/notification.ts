import type { NotificationType } from './enums'

export interface Notification {
  id: string
  message: string
  type: NotificationType
  isRead: boolean
  createdAt: string
}
