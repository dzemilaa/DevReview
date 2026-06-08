import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { officeHoursApi } from '@/api'
import { Spinner } from '@/components/common/Spinner'
import { EmptyState } from '@/components/common/EmptyState'
import { CreateOfficeHourForm } from '@/components/officeHours/CreateOfficeHourForm'
import { OfficeHourCard } from '@/components/officeHours/OfficeHourCard'
import { useOfficeHourActions } from '@/hooks/useOfficeHourActions'
import { OfficeHourStatus } from '@/types'

export function MentorOfficeHoursPage() {
  const queryClient = useQueryClient()
  const { impressions, onCancel, onFeedback, isActionLoading } = useOfficeHourActions()

  const { data, isLoading, error } = useQuery({
    queryKey: ['office-hours', 'mentor-schedule'],
    queryFn: officeHoursApi.getMentorSchedule,
  })

  const createMutation = useMutation({
    mutationFn: officeHoursApi.create,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['office-hours'] })
    },
  })

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">My office hours</h1>
        <p className="mt-1 text-sm text-slate-500">Create and manage your mentoring slots</p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        <CreateOfficeHourForm
          onSubmit={async (payload) => {
            await createMutation.mutateAsync(payload)
          }}
          isSubmitting={createMutation.isPending}
        />

        <div className="space-y-4">
          <h2 className="text-lg font-semibold text-slate-900">Your schedule</h2>

          {error && (
            <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">
              Failed to load schedule.
            </p>
          )}

          {isLoading ? (
            <Spinner />
          ) : !data?.length ? (
            <EmptyState
              title="No slots yet"
              description="Create your first office hour slot using the form."
            />
          ) : (
            <div className="space-y-4">
              {data.map((slot) => (
                <OfficeHourCard
                  key={slot.id}
                  officeHour={slot}
                  showBookerInfo
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
        </div>
      </div>
    </div>
  )
}
