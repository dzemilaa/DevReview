import { Link } from 'react-router-dom'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { officeHoursApi } from '@/api'
import { Button } from '@/components/common/Button'
import { Spinner } from '@/components/common/Spinner'
import { EmptyState } from '@/components/common/EmptyState'
import { OfficeHourCard } from '@/components/officeHours/OfficeHourCard'
import { useAuth } from '@/hooks/useAuth'
import { useOfficeHourActions } from '@/hooks/useOfficeHourActions'
import { OfficeHourStatus } from '@/types'
import { ROUTES } from '@/utils/constants'

export function OfficeHoursPage() {
  const { isAuthenticated, isMentor, user } = useAuth()
  const queryClient = useQueryClient()
  const { impressions, onCancel, onFeedback, isActionLoading } = useOfficeHourActions()

  const { data, isLoading, error } = useQuery({
    queryKey: ['office-hours', 'available'],
    queryFn: officeHoursApi.getAvailable,
  })

  const bookingsQuery = useQuery({
    queryKey: ['office-hours', 'my-bookings'],
    queryFn: officeHoursApi.getMyBookings,
    enabled: isAuthenticated,
  })

  const bookMutation = useMutation({
    mutationFn: ({ id, description }: { id: string; description: string }) =>
      officeHoursApi.book(id, { bookingDescription: description || undefined }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['office-hours'] })
      void queryClient.invalidateQueries({ queryKey: ['notifications'] })
    },
  })

  const activeBookings =
    bookingsQuery.data?.filter((s) => s.status !== OfficeHourStatus.Cancelled) ?? []

  return (
    <div className="space-y-10">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Office hours</h1>
          <p className="mt-1 text-sm text-slate-500">Book a 1-on-1 session with a mentor</p>
        </div>
        {isMentor && (
          <Link to={ROUTES.officeHoursSchedule}>
            <Button variant="secondary">Manage my schedule</Button>
          </Link>
        )}
      </div>

      {isAuthenticated && (
        <section className="space-y-4">
          <h2 className="text-lg font-semibold text-slate-900">My bookings</h2>
          {bookingsQuery.isLoading ? (
            <Spinner />
          ) : activeBookings.length === 0 ? (
            <EmptyState title="No bookings" description="Book an available slot below." />
          ) : (
            <div className="grid gap-4 sm:grid-cols-2">
              {activeBookings.map((slot) => (
                <OfficeHourCard
                  key={slot.id}
                  officeHour={slot}
                  showMentorInfo
                  showCancel={
                    slot.status === OfficeHourStatus.Available ||
                    slot.status === OfficeHourStatus.Booked
                  }
                  showFeedback={slot.status === OfficeHourStatus.Booked}
                  existingImpression={impressions[slot.id] ?? null}
                  onCancel={onCancel}
                  onFeedback={onFeedback}
                  isActionLoading={isActionLoading}
                />
              ))}
            </div>
          )}
        </section>
      )}

      <section className="space-y-4">
        <h2 className="text-lg font-semibold text-slate-900">Available slots</h2>
        {isLoading ? (
          <Spinner />
        ) : error ? (
          <p className="text-sm text-red-600">Could not load office hours.</p>
        ) : !data?.length ? (
          <EmptyState title="No slots available" description="Check back later for new sessions." />
        ) : (
          <div className="grid gap-4 sm:grid-cols-2">
            {data.map((slot) => (
              <OfficeHourCard
                key={slot.id}
                officeHour={slot}
                showMentorInfo
                showBook={isAuthenticated && slot.mentorId !== user?.id}
                onBook={async (slotId, description) => {
                  await bookMutation.mutateAsync({ id: slotId, description })
                }}
                isActionLoading={bookMutation.isPending}
              />
            ))}
          </div>
        )}
      </section>
    </div>
  )
}
