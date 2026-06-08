import { useParams, Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { mentorsApi } from '@/api'
import { Card } from '@/components/common/Card'
import { Spinner } from '@/components/common/Spinner'
import { UserAvatar } from '@/components/users/UserAvatar'
import { ReviewRequestStatusBadge } from '@/components/reviewRequests/ReviewRequestStatusBadge'
import { formatDate, formatDifficulty, formatCurrency } from '@/utils/format'
import { isKnownLanguage } from '@/utils/languages'
import { ROUTES } from '@/utils/constants'

function StarRating({ score }: { score: number }) {
  return (
    <div className="flex items-center gap-0.5">
      {[1, 2, 3, 4, 5].map((i) => (
        <svg
          key={i}
          className={`h-4 w-4 ${i <= Math.round(score) ? 'text-amber-400' : 'text-slate-200'}`}
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
        </svg>
      ))}
    </div>
  )
}

export function MentorProfilePage() {
  const { id } = useParams<{ id: string }>()

  const { data, isLoading, error } = useQuery({
    queryKey: ['mentor', 'profile', id],
    queryFn: () => mentorsApi.getProfile(id!),
    enabled: !!id,
  })

  if (isLoading) return <div className="py-12 flex justify-center"><Spinner /></div>
  if (error || !data) return (
    <div className="py-12 text-center">
      <p className="text-slate-500">Mentor not found.</p>
      <Link to={ROUTES.mentors} className="mt-4 inline-block text-sm text-brand-600 hover:underline">
        Back to mentors
      </Link>
    </div>
  )

  const langExpertise = data.languagesExpertise.filter(l => isKnownLanguage(l.language))

  return (
    <div className="space-y-6">
      <Link to={ROUTES.mentors} className="inline-flex items-center gap-1 text-sm text-slate-500 hover:text-slate-700">
        <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
          <path strokeLinecap="round" strokeLinejoin="round" d="M15 19l-7-7 7-7" />
        </svg>
        Back to mentors
      </Link>

      {/* Header */}
      <Card>
        <div className="flex flex-wrap items-start gap-4">
          <UserAvatar name={data.displayName} size="lg" />
          <div className="min-w-0 flex-1">
            <h1 className="text-xl font-bold text-slate-900">{data.displayName}</h1>
            <p className="text-sm text-slate-500">@{data.userName}</p>
            {data.bio && <p className="mt-2 text-sm text-slate-600">{data.bio}</p>}
            {data.gitHubUrl && (
              <a
                href={data.gitHubUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="mt-1 inline-flex items-center gap-1 text-sm text-brand-600 hover:underline"
              >
                GitHub profile
              </a>
            )}
          </div>
        </div>

        <dl className="mt-5 grid grid-cols-2 gap-4 sm:grid-cols-4 text-sm border-t border-slate-100 pt-5">
          <div>
            <dt className="text-slate-500">Rating</dt>
            <dd className="mt-1 flex items-center gap-1.5">
              <StarRating score={data.averageMentorRating} />
              <span className="font-semibold text-slate-800">{data.averageMentorRating.toFixed(1)}</span>
            </dd>
          </div>
          <div>
            <dt className="text-slate-500">Reviews done</dt>
            <dd className="mt-1 font-semibold text-slate-800">{data.totalReviews}</dd>
          </div>
          <div>
            <dt className="text-slate-500">Experience</dt>
            <dd className="mt-1 font-semibold text-slate-800">{data.yearsOfExperience} yrs</dd>
          </div>
          <div>
            <dt className="text-slate-500">Hourly rate</dt>
            <dd className="mt-1 font-semibold text-slate-800">
              {data.hourlyRate > 0 ? formatCurrency(data.hourlyRate) : 'Free'}
            </dd>
          </div>
          {data.availableHoursPerWeek > 0 && (
            <div>
              <dt className="text-slate-500">Availability</dt>
              <dd className="mt-1 font-semibold text-slate-800">{data.availableHoursPerWeek}h/week</dd>
            </div>
          )}
        </dl>

        {/* Languages */}
        {(langExpertise.length > 0 || data.profileLanguages.length > 0) && (
          <div className="mt-4 border-t border-slate-100 pt-4">
            <p className="mb-2 text-xs font-medium uppercase tracking-wide text-slate-500">Languages & frameworks</p>
            <div className="flex flex-wrap gap-1.5">
              {langExpertise.length > 0
                ? langExpertise.map((l) => (
                    <span key={l.language} className="rounded-md bg-violet-100 px-2 py-0.5 text-xs text-violet-700">
                      {l.language} ({l.score.toFixed(1)}★)
                    </span>
                  ))
                : data.profileLanguages.map((l) => (
                    <span key={l} className="rounded-md bg-slate-100 px-2 py-0.5 text-xs text-slate-700">
                      {l}
                    </span>
                  ))}
            </div>
          </div>
        )}
      </Card>

      {/* Completed public reviews */}
      <section className="space-y-3">
        <h2 className="text-lg font-semibold text-slate-900">Completed reviews</h2>
        {data.publicReviews.length === 0 ? (
          <p className="text-sm text-slate-500">No public completed reviews yet.</p>
        ) : (
          <div className="grid gap-3 sm:grid-cols-2">
            {data.publicReviews.map((r) => (
              <Link key={r.id} to={ROUTES.reviewDetail(r.id)}>
                <Card className="h-full transition-shadow hover:shadow-md">
                  <div className="flex items-start justify-between gap-2">
                    <h3 className="font-medium text-slate-900 line-clamp-2">{r.title}</h3>
                    <ReviewRequestStatusBadge status={r.status} />
                  </div>
                  <p className="mt-1 text-xs text-slate-500">
                    {r.programmingLanguage}
                    {r.framework ? ` · ${r.framework}` : ''}
                    {' · '}{formatDifficulty(r.difficulty)}
                  </p>
                  <p className="mt-2 text-sm text-slate-600 line-clamp-2">{r.description}</p>
                  <p className="mt-3 text-xs text-slate-400">{formatDate(r.createdAt)}</p>
                </Card>
              </Link>
            ))}
          </div>
        )}
      </section>
    </div>
  )
}
