import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { mentorsApi, searchApi } from '@/api'
import { Button } from '@/components/common/Button'
import { Input } from '@/components/common/Input'
import { Pagination } from '@/components/common/Pagination'
import { Spinner } from '@/components/common/Spinner'
import { EmptyState } from '@/components/common/EmptyState'
import { MentorCard } from '@/components/users/MentorCard'
import { useAuth } from '@/hooks/useAuth'
import type { SearchMentorsParams } from '@/types'

type ViewMode = 'top' | 'search' | 'following'

export function MentorsPage() {
  const { isAuthenticated } = useAuth()
  const queryClient = useQueryClient()
  const [mode, setMode] = useState<ViewMode>('top')
  const [topPeriod, setTopPeriod] = useState<'all' | 'week'>('all')
  const [searchParams, setSearchParams] = useState<SearchMentorsParams>({ page: 1, pageSize: 12 })
  const [appliedSearch, setAppliedSearch] = useState<SearchMentorsParams>({ page: 1, pageSize: 12 })

  const topQuery = useQuery({
    queryKey: ['mentors', 'top', topPeriod],
    queryFn: () => mentorsApi.getTop({ period: topPeriod === 'week' ? 'week' : undefined }),
    enabled: mode === 'top',
  })

  const searchQuery = useQuery({
    queryKey: ['search', 'mentors', appliedSearch],
    queryFn: () => searchApi.searchMentors(appliedSearch),
    enabled: mode === 'search',
  })

  const followingQuery = useQuery({
    queryKey: ['mentors', 'following'],
    queryFn: mentorsApi.getFollowing,
    enabled: isAuthenticated,
  })

  const followingDetailsQuery = useQuery({
    queryKey: ['mentors', 'following', 'details'],
    queryFn: mentorsApi.getFollowingDetails,
    enabled: isAuthenticated && mode === 'following',
  })

  const followMutation = useMutation({
    mutationFn: mentorsApi.follow,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['mentors', 'following'] })
    },
  })

  const unfollowMutation = useMutation({
    mutationFn: mentorsApi.unfollow,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['mentors', 'following'] })
    },
  })

  const followingIds = new Set(followingQuery.data ?? [])

  const activeQuery = mode === 'top' ? topQuery : mode === 'search' ? searchQuery : followingDetailsQuery
  const mentors = mode === 'search' ? searchQuery.data?.items : activeQuery.data
  const { isLoading, error } = activeQuery

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Mentors</h1>
        <p className="mt-1 text-sm text-slate-500">Find experienced reviewers for your code</p>
      </div>

      <div className="flex flex-wrap gap-2">
        <Button variant={mode === 'search' ? 'primary' : 'secondary'} size="sm" onClick={() => setMode('search')}>
          Search
        </Button>
        <Button variant={mode === 'top' ? 'primary' : 'secondary'} size="sm" onClick={() => setMode('top')}>
          Top mentors
        </Button>
        {isAuthenticated && (
          <Button variant={mode === 'following' ? 'primary' : 'secondary'} size="sm" onClick={() => setMode('following')}>
            Following
            {followingIds.size > 0 && (
              <span className="ml-1.5 rounded-full bg-white/30 px-1.5 py-0.5 text-xs font-semibold">
                {followingIds.size}
              </span>
            )}
          </Button>
        )}
      </div>

      {mode === 'top' && (
        <div className="flex flex-wrap gap-2">
          <Button
            type="button"
            variant={topPeriod === 'all' ? 'primary' : 'secondary'}
            size="sm"
            onClick={() => setTopPeriod('all')}
          >
            All time
          </Button>
          <Button
            type="button"
            variant={topPeriod === 'week' ? 'primary' : 'secondary'}
            size="sm"
            onClick={() => setTopPeriod('week')}
          >
            This week
          </Button>
        </div>
      )}

      {mode === 'search' && (
        <form
          className="grid gap-4 rounded-xl border border-slate-200 bg-white p-4 sm:grid-cols-2"
          onSubmit={(e) => {
            e.preventDefault()
            setAppliedSearch({ ...searchParams, page: 1 })
          }}
        >
          <Input
            label="Name / username"
            placeholder="e.g. John, john123..."
            value={searchParams.query ?? ''}
            onChange={(e) => setSearchParams((p) => ({ ...p, query: e.target.value || undefined }))}
          />
          <Input
            label="Language"
            value={searchParams.language ?? ''}
            onChange={(e) => setSearchParams((p) => ({ ...p, language: e.target.value || undefined }))}
          />
          <Input
            label="Minimum rating"
            type="number"
            min={0}
            max={5}
            step={0.1}
            value={searchParams.minimumRating ?? ''}
            onChange={(e) =>
              setSearchParams((p) => ({
                ...p,
                minimumRating: e.target.value ? Number(e.target.value) : undefined,
              }))
            }
          />
          <Input
            label="Min. years of experience"
            type="number"
            min={0}
            value={searchParams.minimumYearsOfExperience ?? ''}
            onChange={(e) =>
              setSearchParams((p) => ({
                ...p,
                minimumYearsOfExperience: e.target.value ? Number(e.target.value) : undefined,
              }))
            }
          />
          <Input
            label="Max. hourly rate (€)"
            type="number"
            min={0}
            step={1}
            value={searchParams.maxHourlyRate ?? ''}
            onChange={(e) =>
              setSearchParams((p) => ({
                ...p,
                maxHourlyRate: e.target.value ? Number(e.target.value) : undefined,
              }))
            }
          />
          <div className="sm:col-span-2">
            <Button type="submit">Search mentors</Button>
          </div>
        </form>
      )}

      {error && (
        <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">Failed to load mentors.</p>
      )}

      {mode === 'following' && !isLoading && followingIds.size === 0 ? (
        <EmptyState
          title="Not following anyone yet"
          description="Browse top mentors or search and follow the ones you like."
        />
      ) : isLoading ? (
        <Spinner />
      ) : !mentors?.length ? (
        <EmptyState title="No mentors found" description="Try adjusting your filters." />
      ) : (
        <div className="grid gap-4 sm:grid-cols-2">
          {mentors.map((mentor) => (
            <MentorCard
              key={mentor.mentorId}
              mentor={mentor}
              isFollowing={followingIds.has(mentor.mentorId)}
              onFollow={(id) => followMutation.mutate(id)}
              onUnfollow={(id) => unfollowMutation.mutate(id)}
              isFollowLoading={followMutation.isPending || unfollowMutation.isPending}
            />
          ))}
        </div>
      )}

      {mode === 'search' && searchQuery.data && (
        <Pagination
          currentPage={searchQuery.data.currentPage}
          totalPages={searchQuery.data.totalPages}
          onPageChange={(page) => setAppliedSearch((prev) => ({ ...prev, page }))}
        />
      )}
    </div>
  )
}
