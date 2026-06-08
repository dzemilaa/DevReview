
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { profilesApi, reviewRequestsApi } from '@/api'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Input } from '@/components/common/Input'
import { Spinner } from '@/components/common/Spinner'
import { Textarea } from '@/components/common/Textarea'
import { UserAvatar } from '@/components/users/UserAvatar'
import { ReviewRequestStatusBadge } from '@/components/reviewRequests/ReviewRequestStatusBadge'
import type { UpdateProfilePayload } from '@/types'
import { ROUTES } from '@/utils/constants'
import { formatDate, formatDifficulty } from '@/utils/format'
import { isKnownLanguage } from '@/utils/languages'

const PROFICIENCY_LABELS: Record<number, { label: string; color: string }> = {
  1: { label: 'Beginner', color: 'bg-slate-100 text-slate-600' },
  2: { label: 'Intermediate', color: 'bg-blue-100 text-blue-700' },
  3: { label: 'Advanced', color: 'bg-purple-100 text-purple-700' },
  4: { label: 'Expert', color: 'bg-brand-100 text-brand-700' },
}

function StarRating({ score }: { score: number }) {
  return (
    <div className="flex items-center gap-1">
      {[1, 2, 3, 4, 5].map((star) => (
        <svg
          key={star}
          className={`h-3.5 w-3.5 ${score >= star ? 'text-amber-400' : 'text-slate-200'}`}
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
        </svg>
      ))}
      <span className="ml-1 text-xs font-semibold text-slate-700">{score.toFixed(1)}</span>
    </div>
  )
}

export function ProfilePage() {
  const queryClient = useQueryClient()
  const [editing, setEditing] = useState(false)

  const { data, isLoading, error } = useQuery({
    queryKey: ['profile', 'me'],
    queryFn: profilesApi.getCurrent,
  })

  const isMentor = data?.role === 'Mentor'

  const { data: claimedReviews, isLoading: claimedLoading, error: claimedError } = useQuery({
    queryKey: ['review-requests', 'claimed-by-me'],
    queryFn: reviewRequestsApi.getClaimedByMe,
    enabled: isMentor,
  })

  const [form, setForm] = useState<UpdateProfilePayload | null>(null)

  const updateMutation = useMutation({
    mutationFn: profilesApi.update,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['profile', 'me'] })
      setEditing(false)
      setForm(null)
      window.scrollTo({ top: 0, behavior: 'smooth' })
    },
    onError: (error) => {
      console.error('Profile update failed:', error)
    },
  })

  if (isLoading) return <Spinner />
  if (error || !data) {
    return (
      <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">Could not load profile.</p>
    )
  }

  const startEdit = () => {
    setForm({
      displayName: data.displayName,
      bio: data.bio ?? '',
      gitHubUrl: data.gitHubUrl ?? '',
      yearsOfExperience: data.yearsOfExperience,
      hourlyRate: data.hourlyRate ?? 0,
      availableHoursPerWeek: data.availableHoursPerWeek ?? 0,
      userLanguages: data.userLanguages ?? [],
    })
    setEditing(true)
  }

  const current = form ?? {
    displayName: data.displayName,
    bio: data.bio ?? '',
    gitHubUrl: data.gitHubUrl ?? '',
    yearsOfExperience: data.yearsOfExperience,
    hourlyRate: data.hourlyRate ?? 0,
    availableHoursPerWeek: data.availableHoursPerWeek ?? 0,
    userLanguages: data.userLanguages ?? [],
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div className="flex items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-slate-900">My profile</h1>
        {!editing ? (
          <Button variant="secondary" size="sm" onClick={startEdit}>
            Edit profile
          </Button>
        ) : (
          <Button variant="ghost" size="sm" onClick={() => { setEditing(false); setForm(null) }}>
            Cancel
          </Button>
        )}
      </div>

      {/* --- PROFILE CARD --- */}
      <Card>
        <div className="flex items-center gap-4">
          <UserAvatar name={data.displayName} size="lg" />
          <div>
            <h2 className="text-xl font-semibold text-slate-900">{data.displayName}</h2>
            <p className="text-sm text-slate-500">@{data.userName}</p>
            <span
              className={`mt-1 inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${
                data.role === 'Admin'
                  ? 'bg-red-100 text-red-700'
                  : data.role === 'Mentor'
                  ? 'bg-brand-100 text-brand-700'
                  : 'bg-emerald-100 text-emerald-700'
              }`}
            >
              {data.role}
            </span>
          </div>
        </div>

        {/* EDIT FORM */}
        {editing ? (
          <form
            className="mt-6 space-y-4"
            onSubmit={(e) => {
              e.preventDefault()
              updateMutation.mutate(current)
            }}
          >
            <Input
              label="Display name"
              value={current.displayName}
              onChange={(e) => setForm({ ...current, displayName: e.target.value })}
              required
            />

            {isMentor && (
              <>
                <Textarea
                  label="Bio"
                  value={current.bio ?? ''}
                  onChange={(e) => setForm({ ...current, bio: e.target.value })}
                  rows={3}
                />
                <Input
                  label="GitHub URL"
                  value={current.gitHubUrl ?? ''}
                  onChange={(e) => setForm({ ...current, gitHubUrl: e.target.value })}
                />
                <div className="grid grid-cols-3 gap-4">
                  <Input
                    label="Years of experience"
                    type="number"
                    min={0}
                    value={current.yearsOfExperience}
                    onChange={(e) =>
                      setForm({ ...current, yearsOfExperience: Number(e.target.value) })
                    }
                  />
                  <Input
                    label="Hourly rate (€)"
                    type="number"
                    min={0}
                    step="0.01"
                    value={current.hourlyRate}
                    onChange={(e) => setForm({ ...current, hourlyRate: Number(e.target.value) })}
                  />
                  <Input
                    label="Available hours/week"
                    type="number"
                    min={0}
                    value={current.availableHoursPerWeek}
                    onChange={(e) =>
                      setForm({ ...current, availableHoursPerWeek: Number(e.target.value) })
                    }
                  />
                </div>

                <div className="space-y-3 rounded-lg border border-slate-200 bg-slate-50 p-4">
                  <label className="block text-sm font-semibold text-slate-700">
                    Languages & frameworks
                  </label>
                  {current.userLanguages.map((ul, idx) => (
                    <div key={idx} className="flex items-center gap-2">
                      <input
                        className="flex-1 rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/20"
                        placeholder="Language or framework (e.g. React)"
                        value={ul.language}
                        onChange={(e) => {
                          const next = [...current.userLanguages]
                          next[idx] = { ...next[idx], language: e.target.value }
                          setForm({ ...current, userLanguages: next })
                        }}
                      />
                      <select
                        className="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/20"
                        value={ul.proficiency}
                        onChange={(e) => {
                          const next = [...current.userLanguages]
                          next[idx] = { ...next[idx], proficiency: Number(e.target.value) }
                          setForm({ ...current, userLanguages: next })
                        }}
                      >
                        <option value={1}>Beginner</option>
                        <option value={2}>Intermediate</option>
                        <option value={3}>Advanced</option>
                        <option value={4}>Expert</option>
                      </select>
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        onClick={() => {
                          const next = current.userLanguages.filter((_, i) => i !== idx)
                          setForm({ ...current, userLanguages: next })
                        }}
                      >
                        ✕
                      </Button>
                    </div>
                  ))}
                  <Button
                    type="button"
                    variant="secondary"
                    size="sm"
                    onClick={() =>
                      setForm({
                        ...current,
                        userLanguages: [...current.userLanguages, { language: '', proficiency: 2 }],
                      })
                    }
                  >
                    + Add language
                  </Button>
                </div>
              </>
            )}

            <Button type="submit" isLoading={updateMutation.isPending}>
              Save changes
            </Button>
          </form>
        ) : (
          // VIEW MODE
          <div className="mt-6 space-y-4">
            <dl className="grid grid-cols-2 gap-3 text-sm">
              <div className="rounded-lg bg-slate-50 px-3 py-2">
                <dt className="text-xs text-slate-500">Username</dt>
                <dd className="mt-0.5 font-medium text-slate-800">@{data.userName}</dd>
              </div>
              <div className="rounded-lg bg-slate-50 px-3 py-2">
                <dt className="text-xs text-slate-500">Email</dt>
                <dd className="mt-0.5 font-medium text-slate-800 truncate">{data.email}</dd>
              </div>
            </dl>

            {isMentor && (
              <>
                {data.bio && <p className="text-sm text-slate-600">{data.bio}</p>}
                {data.gitHubUrl && (
                  <a
                    href={data.gitHubUrl}
                    target="_blank"
                    rel="noreferrer"
                    className="inline-flex items-center gap-1.5 text-sm text-brand-600 hover:underline"
                  >
                    <svg className="h-4 w-4" fill="currentColor" viewBox="0 0 24 24">
                      <path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0024 12c0-6.63-5.37-12-12-12z" />
                    </svg>
                    GitHub
                  </a>
                )}
                <dl className="grid grid-cols-2 gap-3 text-sm sm:grid-cols-3">
                  <div className="rounded-lg bg-slate-50 px-3 py-2">
                    <dt className="text-xs text-slate-500">Experience</dt>
                    <dd className="mt-0.5 font-medium text-slate-800">{data.yearsOfExperience} yrs.</dd>
                  </div>
                  {(data.hourlyRate ?? 0) > 0 && (
                    <div className="rounded-lg bg-amber-50 px-3 py-2">
                      <dt className="text-xs text-amber-600">Hourly rate</dt>
                      <dd className="mt-0.5 font-medium text-amber-800">€{data.hourlyRate?.toFixed(2)}</dd>
                    </div>
                  )}
                  {(data.availableHoursPerWeek ?? 0) > 0 && (
                    <div className="rounded-lg bg-slate-50 px-3 py-2">
                      <dt className="text-xs text-slate-500">Availability</dt>
                      <dd className="mt-0.5 font-medium text-slate-800">{data.availableHoursPerWeek} h/week</dd>
                    </div>
                  )}
                  <div className="rounded-lg bg-slate-50 px-3 py-2">
                    <dt className="text-xs text-slate-500">Total reviews</dt>
                    <dd className="mt-0.5 font-medium text-slate-800">{data.totalReviews}</dd>
                  </div>
                </dl>
              </>
            )}
          </div>
        )}
      </Card>

      {/* --- MY LANGUAGES & FRAMEWORKS (mentor only) --- */}
      {!editing && isMentor && data.userLanguages && data.userLanguages.length > 0 && (
        <Card>
          <h3 className="mb-3 text-sm font-semibold text-slate-700">Languages & frameworks</h3>
          <div className="flex flex-wrap gap-2">
            {data.userLanguages.map((ul) => {
              const info = PROFICIENCY_LABELS[ul.proficiency] ?? { label: 'Unknown', color: 'bg-slate-100 text-slate-600' }
              return (
                <span
                  key={ul.language}
                  className={`inline-flex items-center gap-1.5 rounded-full px-3 py-1.5 text-sm font-medium ${info.color}`}
                >
                  {ul.language}
                  <span className="rounded-full bg-white/60 px-1.5 py-0.5 text-xs">
                    {info.label}
                  </span>
                </span>
              )
            })}
          </div>
        </Card>
      )}

      {/* --- MENTOR REPUTATION --- */}
      {!editing && isMentor && (
        <Card>
          <div className="flex items-center justify-between">
            <h3 className="text-sm font-semibold text-slate-700">Mentor reputation</h3>
            {data.totalReviews > 0 && (
              <div className="flex items-center gap-2">
                <StarRating score={data.averageMentorRating} />
                <span className="text-xs text-slate-500">
                  ({data.totalReviews} {data.totalReviews === 1 ? 'rating' : 'ratings'})
                </span>
              </div>
            )}
          </div>

          {data.totalReviews === 0 ? (
            <p className="mt-3 text-sm text-slate-500">
              No ratings yet. Ratings are added when an author rates a completed review.
            </p>
          ) : (
            <>
              <div className="mt-4 flex items-center gap-3">
                <span className="text-sm text-slate-500">Average rating</span>
                <StarRating score={data.averageMentorRating} />
                <span className="text-sm font-semibold text-slate-700">{data.averageMentorRating.toFixed(2)} / 5.00</span>
              </div>

              {data.languagesExpertise && data.languagesExpertise.filter(l => isKnownLanguage(l.language)).length > 0 && (
                <div className="mt-4 space-y-4">
                  <p className="text-xs font-medium text-slate-500 uppercase tracking-wide">By language</p>
                  {data.languagesExpertise.filter(l => isKnownLanguage(l.language)).map((lang) => (
                    <div key={lang.language}>
                      <div className="flex items-center gap-3 mb-2">
                        <span className="w-24 shrink-0 rounded-md bg-slate-100 px-2 py-0.5 text-center text-xs font-medium text-slate-700 truncate">
                          {lang.language}
                        </span>
                        <StarRating score={lang.score} />
                        <span className="text-xs text-slate-500">
                          {lang.totalReviews} {lang.totalReviews === 1 ? 'review' : 'reviews'}
                        </span>
                      </div>
                      {lang.totalReviews > 0 && (
                        <div className="ml-27 flex gap-3 text-xs text-slate-500">
                          {([5, 4, 3, 2, 1] as const).map((star) => {
                            const count = lang[`score${star}Count` as keyof typeof lang] as number
                            return (
                              <span key={star} className="flex flex-col items-center gap-0.5">
                                <span className="font-semibold text-slate-700">{count}</span>
                                <span>{star}★</span>
                              </span>
                            )
                          })}
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </>
          )}
        </Card>
      )}

      {/* --- CLAIMED REVIEWS (mentor only) --- */}
      {!editing && isMentor && (
        <Card>
          <h3 className="mb-3 text-sm font-semibold text-slate-700">My claimed reviews</h3>
          {claimedLoading ? (
            <Spinner />
          ) : claimedError ? (
            <p className="text-sm text-red-600">Failed to load claimed reviews.</p>
          ) : !claimedReviews || claimedReviews.length === 0 ? (
            <p className="text-sm text-slate-500">You have not claimed any review requests yet.</p>
          ) : (
            <ul className="divide-y divide-slate-100">
              {claimedReviews.map((r) => (
                <li key={r.id}>
                  <Link
                    to={ROUTES.reviewDetail(r.id)}
                    className="flex items-center justify-between gap-4 py-3 hover:bg-slate-50 -mx-1 px-1 rounded-lg transition"
                  >
                    <div className="min-w-0">
                      <p className="truncate text-sm font-medium text-slate-900">{r.title}</p>
                      <p className="mt-0.5 text-xs text-slate-500">
                        {r.programmingLanguage}
                        {r.framework ? ` · ${r.framework}` : ''} · {formatDifficulty(r.difficulty)} · {formatDate(r.createdAt)}
                      </p>
                    </div>
                    <ReviewRequestStatusBadge status={r.status} />
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </Card>
      )}
    </div>
  )
}