import { Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { reviewRequestsApi } from '@/api'
import { Card } from '@/components/common/Card'
import { EmptyState } from '@/components/common/EmptyState'
import { Spinner } from '@/components/common/Spinner'
import { ReviewRequestStatusBadge } from '@/components/reviewRequests/ReviewRequestStatusBadge'
import { ROUTES } from '@/utils/constants'
import { formatDate, formatDifficulty } from '@/utils/format'

export function KnowledgeBasePage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['review-requests', 'public'],
    queryFn: () => reviewRequestsApi.getPublic(),
  })

  if (isLoading) return <Spinner />

  if (error) {
    return (
      <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">
        Could not load the knowledge base.
      </p>
    )
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Knowledge base</h1>
        <p className="mt-1 text-sm text-slate-500">
          Public, completed code reviews shared by the community
        </p>
      </div>

      {!data?.length ? (
        <EmptyState
          title="No public reviews yet"
          description="When authors mark reviews as public and mentors complete them, they appear here."
        />
      ) : (
        <ul className="space-y-4">
          {data.map((item) => (
            <li key={item.id}>
              <Card>
                <div className="flex flex-wrap items-start justify-between gap-3">
                  <div>
                    <Link
                      to={ROUTES.reviewDetail(item.id)}
                      className="text-lg font-semibold text-brand-700 hover:underline"
                    >
                      {item.title}
                    </Link>
                    <p className="mt-1 line-clamp-2 text-sm text-slate-600">{item.description}</p>
                    <div className="mt-2 flex flex-wrap gap-2 text-xs text-slate-500">
                      <span>{item.programmingLanguage}</span>
                      {item.framework && <span>· {item.framework}</span>}
                      <span>· {formatDifficulty(item.difficulty)}</span>
                      <span>· {formatDate(item.createdAt)}</span>
                    </div>
                  </div>
                  <ReviewRequestStatusBadge status={item.status} />
                </div>
              </Card>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
