import { useMemo } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { reviewRequestsApi } from '@/api'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Spinner } from '@/components/common/Spinner'
import { CommentForm } from '@/components/comments/CommentForm'
import { CommentList } from '@/components/comments/CommentList'
import { AbandonReviewForm } from '@/components/reviewRequests/AbandonReviewForm'
import { FinalReviewSummaryCard } from '@/components/reviewRequests/FinalReviewSummaryCard'
import { FinalizeReviewForm } from '@/components/reviewRequests/FinalizeReviewForm'
import { InteractiveCodeReview } from '@/components/reviewRequests/InteractiveCodeReview'
import { PeerRatingForm } from '@/components/reviewRequests/PeerRatingForm'
import { ReviewRequestStatusBadge } from '@/components/reviewRequests/ReviewRequestStatusBadge'
import { useAuth } from '@/hooks/useAuth'
import type { AddCommentPayload, Comment } from '@/types'
import { ReviewStatus } from '@/types'
import { formatCurrency, formatDate, formatDifficulty } from '@/utils/format'
import { ROUTES } from '@/utils/constants'

function filterGeneralComments(comments: Comment[]): Comment[] {
  return comments
    .filter((c) => !c.codeFilePath || c.startLine == null)
    .map((c) => ({
      ...c,
      replies: c.replies?.length ? filterGeneralComments(c.replies) : [],
    }))
}

export function ReviewRequestDetailPage() {
  const { id } = useParams<{ id: string }>()
  const queryClient = useQueryClient()
  const { isMentor, isAuthenticated, user } = useAuth()

  const { data, isLoading, error } = useQuery({
    queryKey: ['review-request', id],
    queryFn: () => reviewRequestsApi.getById(id!),
    enabled: Boolean(id),
  })

  const invalidate = () => {
    void queryClient.invalidateQueries({ queryKey: ['review-request', id] })
    void queryClient.invalidateQueries({ queryKey: ['search', 'review-requests'] })
    void queryClient.invalidateQueries({ queryKey: ['review-requests'] })
  }

  const claimMutation = useMutation({
    mutationFn: () => reviewRequestsApi.claim(id!),
    onSuccess: invalidate,
  })

  const abandonMutation = useMutation({
    mutationFn: (reason: string) => reviewRequestsApi.abandon(id!, { reason }),
    onSuccess: invalidate,
  })

  const finalizeMutation = useMutation({
    mutationFn: (payload: {
      summary: string
      priorityFixes: string
      qualityScore: number
    }) => reviewRequestsApi.finalize(id!, payload),
    onSuccess: invalidate,
  })

  const commentMutation = useMutation({
    mutationFn: (payload: AddCommentPayload) => reviewRequestsApi.addComment(id!, payload),
    onSuccess: invalidate,
  })

  const resolveMutation = useMutation({
    mutationFn: (commentId: string) => reviewRequestsApi.resolveComment(id!, commentId),
    onSuccess: invalidate,
  })

  const applyMutation = useMutation({
    mutationFn: (commentId: string) => reviewRequestsApi.applySuggestion(id!, commentId),
    onSuccess: invalidate,
  })

  const ratingMutation = useMutation({
    mutationFn: (rating: number) => reviewRequestsApi.submitPeerRating(id!, rating),
    onSuccess: invalidate,
  })

  const approveMutation = useMutation({
    mutationFn: (approved: boolean) => reviewRequestsApi.approveReview(id!, approved),
    onSuccess: invalidate,
  })

  const generalComments = useMemo(
    () => (data ? filterGeneralComments(data.comments) : []),
    [data],
  )

  if (isLoading) {
    return <Spinner />
  }

  if (error || !data) {
    return (
      <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">
        Review request not found or could not be loaded.
      </p>
    )
  }

  const isAssignedMentor = isMentor && data.mentorId === user?.id
  const canClaim =
    isMentor && data.status === ReviewStatus.Open && data.ownerId !== user?.id
  const canAbandon =
    isAssignedMentor &&
    (data.status === ReviewStatus.Claimed || data.status === ReviewStatus.InReview)
  const canFinalize =
    isAssignedMentor &&
    (data.status === ReviewStatus.Claimed || data.status === ReviewStatus.InReview)
  const canComment =
    isAssignedMentor &&
    (data.status === ReviewStatus.Claimed || data.status === ReviewStatus.InReview)
  const canGeneralComment = isAuthenticated
  const isCompleted = data.status === ReviewStatus.Completed
  const isPendingApproval = data.status === ReviewStatus.PendingApproval
  const isOwner = user?.id === data.ownerId
  const canApproveReview = isOwner && isPendingApproval
  const alreadyRatedMentor = data.finalReview?.authorRatingOfMentor != null
  const alreadyRatedAuthor = data.finalReview?.mentorRatingOfAuthor != null
  const canRateMentor = isOwner && isCompleted && !alreadyRatedMentor
  const canRateAuthor = isAssignedMentor && isCompleted && !alreadyRatedAuthor

  const rootComments = data.comments.filter((c) => c.parentCommentId == null)
  const resolvedCount = rootComments.filter((c) => c.isResolved).length
  const totalComments = rootComments.length
  const resolutionPct = totalComments === 0 ? 100 : Math.round((resolvedCount / totalComments) * 100)
  const canFinalizeSafely = resolutionPct >= 80
  const canApplySuggestions = isOwner && !isCompleted
  const codeFiles = data.codeFiles ?? []

  return (
    <div className="space-y-8">
      <header className="space-y-4">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-slate-900">{data.title}</h1>
            <p className="mt-2 text-slate-600">{data.description}</p>
          </div>
          <ReviewRequestStatusBadge status={data.status} />
        </div>

        <div className="flex flex-wrap gap-2 text-sm text-slate-600">
          <span className="rounded-md bg-slate-100 px-2 py-1">{data.programmingLanguage}</span>
          {data.framework && (
            <span className="rounded-md bg-slate-100 px-2 py-1">{data.framework}</span>
          )}
          <span className="rounded-md bg-brand-50 px-2 py-1 text-brand-700">
            {formatDifficulty(data.difficulty)}
          </span>
          {data.isPaid && data.price > 0 && (
            <span className="rounded-md bg-amber-50 px-2 py-1 text-amber-800">
              {formatCurrency(data.price)}
            </span>
          )}
          <span className="text-slate-400">· {formatDate(data.createdAt)}</span>
        </div>

        {data.tags.length > 0 && (
          <div className="flex flex-wrap gap-1.5">
            {data.tags.map((tag) => (
              <span key={tag} className="text-sm text-slate-500">
                #{tag}
              </span>
            ))}
          </div>
        )}

        {canClaim && (
          <Button onClick={() => claimMutation.mutate()} isLoading={claimMutation.isPending}>
            Claim for review
          </Button>
        )}
      </header>

      {data.finalReview && <FinalReviewSummaryCard review={data.finalReview} />}

      {canApproveReview && (
        <section className="rounded-xl border border-amber-200 bg-amber-50 p-6 space-y-3">
          <h2 className="font-semibold text-amber-900">Review completed — publish to Knowledge Base?</h2>
          <p className="text-sm text-amber-800">
            Your mentor has finished the review. Do you want to publish this review publicly to the Knowledge Base so others can learn from it?
          </p>
          <div className="flex gap-3">
            <Button
              onClick={() => approveMutation.mutate(true)}
              isLoading={approveMutation.isPending}
            >
              Approve & publish
            </Button>
            <Button
              variant="secondary"
              onClick={() => approveMutation.mutate(false)}
              isLoading={approveMutation.isPending}
            >
              Keep private
            </Button>
          </div>
        </section>
      )}

      {(canAbandon || canFinalize) && (
        <div className="grid gap-4 lg:grid-cols-2">
          {canAbandon && (
            <AbandonReviewForm
              onSubmit={async (reason) => {
                await abandonMutation.mutateAsync(reason)
              }}
              isSubmitting={abandonMutation.isPending}
            />
          )}
          {canFinalize && (
            <>
              {totalComments > 0 && (
                <div className={`rounded-lg border px-4 py-3 text-sm ${canFinalizeSafely ? 'border-emerald-200 bg-emerald-50 text-emerald-800' : 'border-amber-200 bg-amber-50 text-amber-800'}`}>
                  <span className="font-semibold">{resolvedCount}/{totalComments} comments resolved ({resolutionPct}%)</span>
                  {!canFinalizeSafely && <span className="ml-2">— need ≥80% to finalize</span>}
                </div>
              )}
              <FinalizeReviewForm
                onSubmit={async (payload) => {
                  await finalizeMutation.mutateAsync(payload)
                }}
                isSubmitting={finalizeMutation.isPending}
              />
            </>
          )}
        </div>
      )}

      {codeFiles.length > 0 ? (
        <InteractiveCodeReview
          codeFiles={codeFiles}
          comments={data.comments}
          canComment={canComment}
          canResolve={isAssignedMentor}
          canApplySuggestions={canApplySuggestions}
          onAddComment={async (payload) => {
            await commentMutation.mutateAsync(payload)
          }}
          onResolve={async (commentId) => {
            await resolveMutation.mutateAsync(commentId)
          }}
          onApplySuggestion={async (commentId) => {
            await applyMutation.mutateAsync(commentId)
          }}
          isSubmitting={commentMutation.isPending}
          isResolving={resolveMutation.isPending}
          isApplying={applyMutation.isPending}
        />
      ) : (
        <section className="rounded-xl border border-dashed border-slate-300 bg-slate-50 p-6 text-center">
          <h2 className="text-lg font-semibold text-slate-900">Code review</h2>
          <p className="mt-2 text-sm text-slate-500">No code files were attached to this request.</p>
        </section>
      )}

      {(canRateMentor || canRateAuthor) && (
        <section className="grid gap-4 lg:grid-cols-2">
          {canRateMentor && (
            <PeerRatingForm
              label="Rate your mentor"
              onSubmit={async (rating) => {
                await ratingMutation.mutateAsync(rating)
              }}
              isSubmitting={ratingMutation.isPending}
            />
          )}
          {canRateAuthor && (
            <PeerRatingForm
              label="Rate the author"
              onSubmit={async (rating) => {
                await ratingMutation.mutateAsync(rating)
              }}
              isSubmitting={ratingMutation.isPending}
            />
          )}
        </section>
      )}

      <section className="space-y-4">
        <h2 className="text-lg font-semibold text-slate-900">General discussion</h2>
        <CommentList
          comments={generalComments}
          canResolve={isAssignedMentor}
          onResolve={async (commentId) => {
            await resolveMutation.mutateAsync(commentId)
          }}
          onReply={isAuthenticated ? async (payload) => {
            await commentMutation.mutateAsync(payload)
          } : undefined}
          isResolving={resolveMutation.isPending}
          isReplying={commentMutation.isPending}
        />
        {canGeneralComment ? (
          <Card>
            <CommentForm
              onSubmit={async (payload) => {
                await commentMutation.mutateAsync(payload)
              }}
              isSubmitting={commentMutation.isPending}
            />
          </Card>
        ) : (
          <p className="text-sm text-slate-500">
            <Link to={ROUTES.login} className="text-brand-600 hover:underline">Sign in</Link> to join the discussion.
          </p>
        )}
      </section>
    </div>
  )
}
