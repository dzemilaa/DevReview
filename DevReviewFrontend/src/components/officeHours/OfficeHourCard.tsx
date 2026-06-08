import { useState } from 'react'
import { Card } from '@/components/common/Card'
import { Button } from '@/components/common/Button'
import { Textarea } from '@/components/common/Textarea'
import { OfficeHourStatusBadge } from './OfficeHourStatusBadge'
import { OfficeHourFeedbackForm } from './OfficeHourFeedbackForm'
import type { OfficeHour, OfficeHourDetail } from '@/types'
import { OfficeHourStatus } from '@/types'
import { formatCurrency, formatDate } from '@/utils/format'

interface OfficeHourCardProps {
  officeHour: OfficeHour | OfficeHourDetail
  showBook?: boolean
  showCancel?: boolean
  showFeedback?: boolean
  showMentorInfo?: boolean
  showBookerInfo?: boolean
  existingImpression?: string | null
  onBook?: (id: string, description: string) => Promise<void>
  onCancel?: (id: string, reason: string) => Promise<void>
  onFeedback?: (id: string, impression: string) => Promise<void>
  isActionLoading?: boolean
}

function isPastEnd(endTime: string): boolean {
  return Date.now() >= new Date(endTime).getTime()
}

function isDetail(oh: OfficeHour | OfficeHourDetail): oh is OfficeHourDetail {
  return 'mentorName' in oh
}

export function OfficeHourCard({
  officeHour,
  showBook = false,
  showCancel = false,
  showFeedback = false,
  showMentorInfo = false,
  showBookerInfo = false,
  existingImpression,
  onBook,
  onCancel,
  onFeedback,
  isActionLoading = false,
}: OfficeHourCardProps) {
  const [bookingDescription, setBookingDescription] = useState('')
  const [cancelReason, setCancelReason] = useState('')
  const [showBookForm, setShowBookForm] = useState(false)
  const [showCancelForm, setShowCancelForm] = useState(false)

  const detail = isDetail(officeHour) ? officeHour : null

  return (
    <Card>
      <div className="flex items-start justify-between gap-3">
        <h3 className="font-semibold text-slate-900">{officeHour.topic}</h3>
        <OfficeHourStatusBadge status={officeHour.status} />
      </div>

      <dl className="mt-4 space-y-2 text-sm">
        {showMentorInfo && detail && (
          <div className="flex justify-between gap-4">
            <dt className="text-slate-500">Mentor</dt>
            <dd className="text-right">
              <span className="font-medium text-slate-800">{detail.mentorName}</span>
              {detail.mentorEmail && (
                <span className="ml-2 text-slate-500">{detail.mentorEmail}</span>
              )}
            </dd>
          </div>
        )}
        {showBookerInfo && detail?.bookedByName && (
          <div className="flex justify-between gap-4">
            <dt className="text-slate-500">Booked by</dt>
            <dd className="text-right">
              <span className="font-medium text-slate-800">{detail.bookedByName}</span>
              {detail.bookedByEmail && (
                <span className="ml-2 text-slate-500">{detail.bookedByEmail}</span>
              )}
            </dd>
          </div>
        )}
        <div className="flex justify-between gap-4">
          <dt className="text-slate-500">Start</dt>
          <dd className="font-medium text-slate-800">{formatDate(officeHour.startTime)}</dd>
        </div>
        <div className="flex justify-between gap-4">
          <dt className="text-slate-500">Duration</dt>
          <dd className="font-medium text-slate-800">{officeHour.durationMinutes} min</dd>
        </div>
        {officeHour.price != null && officeHour.price > 0 && (
          <div className="flex justify-between gap-4">
            <dt className="text-slate-500">Price</dt>
            <dd className="font-medium text-slate-800">{formatCurrency(officeHour.price)}</dd>
          </div>
        )}
        {officeHour.bookingDescription && (
          <div>
            <dt className="text-slate-500">Booking note</dt>
            <dd className="mt-1 text-slate-700">{officeHour.bookingDescription}</dd>
          </div>
        )}
      </dl>

      {showBook && officeHour.status === OfficeHourStatus.Available && onBook && (
        <div className="mt-4 border-t border-slate-100 pt-4">
          {!showBookForm ? (
            <Button size="sm" onClick={() => setShowBookForm(true)}>
              Book session
            </Button>
          ) : (
            <div className="space-y-3">
              <Textarea
                label="What would you like to discuss?"
                value={bookingDescription}
                onChange={(e) => setBookingDescription(e.target.value)}
                rows={3}
              />
              <div className="flex gap-2">
                <Button
                  size="sm"
                  isLoading={isActionLoading}
                  onClick={() => void onBook(officeHour.id, bookingDescription).then(() => setShowBookForm(false))}
                >
                  Confirm booking
                </Button>
                <Button size="sm" variant="ghost" onClick={() => setShowBookForm(false)}>
                  Cancel
                </Button>
              </div>
            </div>
          )}
        </div>
      )}

      {showCancel &&
        (officeHour.status === OfficeHourStatus.Available ||
          officeHour.status === OfficeHourStatus.Booked) &&
        onCancel && (
          <div className="mt-4 border-t border-slate-100 pt-4">
            {!showCancelForm ? (
              <Button size="sm" variant="danger" onClick={() => setShowCancelForm(true)}>
                Cancel session
              </Button>
            ) : (
              <div className="space-y-3">
                <Textarea
                  label="Reason (optional)"
                  value={cancelReason}
                  onChange={(e) => setCancelReason(e.target.value)}
                  rows={2}
                />
                <div className="flex gap-2">
                  <Button
                    size="sm"
                    variant="danger"
                    isLoading={isActionLoading}
                    onClick={() =>
                      void onCancel(officeHour.id, cancelReason).then(() => setShowCancelForm(false))
                    }
                  >
                    Confirm cancel
                  </Button>
                  <Button size="sm" variant="ghost" onClick={() => setShowCancelForm(false)}>
                    Back
                  </Button>
                </div>
              </div>
            )}
          </div>
        )}

      {showBookerInfo && detail?.bookedByEmail &&
        officeHour.status === OfficeHourStatus.Booked && (
          <div className="mt-4 border-t border-slate-100 pt-4 text-sm text-slate-600">
            Send session details to {detail.bookedByName ?? 'attendee'} at{' '}
            <span className="font-medium text-slate-800">{detail.bookedByEmail}</span>
          </div>
        )}

      {showFeedback &&
        isPastEnd(officeHour.endTime) &&
        officeHour.status !== OfficeHourStatus.Cancelled &&
        onFeedback && (
          <div className="mt-4 border-t border-slate-100 pt-4">
            <OfficeHourFeedbackForm
              existingImpression={existingImpression}
              isSubmitting={isActionLoading}
              onSubmit={async (impression) => {
                await onFeedback(officeHour.id, impression)
              }}
            />
          </div>
        )}
    </Card>
  )
}
