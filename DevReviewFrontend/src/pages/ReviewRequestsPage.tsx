import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { searchApi } from '@/api'
import { Button } from '@/components/common/Button'
import { Pagination } from '@/components/common/Pagination'
import { ReviewRequestList } from '@/components/reviewRequests/ReviewRequestList'
import { ReviewSearchFilters } from '@/components/reviewRequests/ReviewSearchFilters'
import type { SearchReviewRequestsParams } from '@/types'
import { ReviewStatus } from '@/types'
import { ROUTES } from '@/utils/constants'

const defaultFilters: SearchReviewRequestsParams = {
  page: 1,
  pageSize: 12,
}

export function ReviewRequestsPage() {
  const [filters, setFilters] = useState<SearchReviewRequestsParams>(defaultFilters)
  const [applied, setApplied] = useState<SearchReviewRequestsParams>(defaultFilters)

  const { data, isLoading, error } = useQuery({
    queryKey: ['search', 'review-requests', applied],
    queryFn: () => searchApi.searchReviewRequests(applied),
  })

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Review requests</h1>
          <p className="mt-1 text-sm text-slate-500">
            Search and filter code review requests
            {data != null && ` · ${data.totalItems} total`}
          </p>
        </div>
        <Link to={ROUTES.newReview}>
          <Button>New request</Button>
        </Link>
      </div>

      <ReviewSearchFilters
        filters={filters}
        onChange={setFilters}
        onSearch={() => setApplied({ ...filters, page: 1 })}
      />

      {error && (
        <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">
          Failed to load review requests. Is the API running?
        </p>
      )}

      <ReviewRequestList
        reviews={data?.items ?? []}
        isLoading={isLoading}
        emptyAction={
          <Link to={ROUTES.newReview}>
            <Button>Create your first request</Button>
          </Link>
        }
      />

      {data && (
        <Pagination
          currentPage={data.currentPage}
          totalPages={data.totalPages}
          onPageChange={(page) => setApplied((prev) => ({ ...prev, page }))}
        />
      )}
    </div>
  )
}
