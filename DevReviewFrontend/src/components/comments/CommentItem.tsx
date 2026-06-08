import { useState } from 'react'
import { Badge } from '@/components/common/Badge'
import { Button } from '@/components/common/Button'
import { CommentForm } from '@/components/comments/CommentForm'
import { SuggestedCodeDiff } from '@/components/comments/SuggestedCodeDiff'
import { UserAvatar } from '@/components/users/UserAvatar'
import type { AddCommentPayload, Comment } from '@/types'
import { commentTypeColor } from '@/utils/codeDiff'
import { formatCommentType, formatDate } from '@/utils/format'
import { cn } from '@/utils/cn'

interface CommentItemProps {
  comment: Comment
  depth?: number
  fileContent?: string
  canResolve?: boolean
  canApplySuggestion?: boolean
  onResolve?: (commentId: string) => Promise<void>
  onReply?: (payload: AddCommentPayload) => Promise<void>
  onApplySuggestion?: (commentId: string) => Promise<void>
  isResolving?: boolean
  isReplying?: boolean
  isApplying?: boolean
}

export function CommentItem({
  comment,
  depth = 0,
  fileContent,
  canResolve = false,
  canApplySuggestion = false,
  onResolve,
  onReply,
  onApplySuggestion,
  isResolving = false,
  isReplying = false,
  isApplying = false,
}: CommentItemProps) {
  const [showReply, setShowReply] = useState(false)
  const replies = comment.replies ?? []

  const hasLineRange = comment.startLine != null
  const endLine = comment.endLine ?? comment.startLine ?? 0

  return (
    <article
      className={cn(
        'rounded-lg border border-l-4 p-4',
        commentTypeColor(comment.type),
        depth === 1 && 'ml-4',
        depth >= 2 && 'ml-8',
      )}
    >
      <div className="flex items-start gap-3">
        <UserAvatar name={comment.authorName} />
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <span className="text-sm font-medium text-slate-800">{comment.authorName}</span>
            <Badge variant="neutral">{formatCommentType(comment.type)}</Badge>
            {comment.isResolved && <Badge variant="success">Resolved</Badge>}
            <span className="text-xs text-slate-400">{formatDate(comment.createdAt)}</span>
          </div>

          {comment.codeFilePath && hasLineRange && (
            <p className="mt-1 text-xs font-medium text-slate-500">
              {comment.codeFilePath} · lines {comment.startLine}–{endLine}
            </p>
          )}

          <p className="mt-2 whitespace-pre-wrap text-sm text-slate-700">{comment.content}</p>

          {comment.suggestedCode && hasLineRange && fileContent && (
            <SuggestedCodeDiff
              fileContent={fileContent}
              startLine={comment.startLine!}
              endLine={endLine}
              suggestedCode={comment.suggestedCode}
              canApply={canApplySuggestion}
              isApplying={isApplying}
              onApply={onApplySuggestion ? () => onApplySuggestion(comment.id) : undefined}
            />
          )}

          {comment.suggestedCode && hasLineRange && !fileContent && (
            <pre className="mt-2 overflow-x-auto rounded-md bg-slate-900 p-3 text-xs text-slate-100">
              <code>{comment.suggestedCode}</code>
            </pre>
          )}

          <div className="mt-2 flex flex-wrap gap-2">
            {canResolve && !comment.isResolved && onResolve && (
              <Button
                type="button"
                variant="secondary"
                size="sm"
                isLoading={isResolving}
                onClick={() => void onResolve(comment.id)}
              >
                Mark resolved
              </Button>
            )}
            {onReply && (
              <Button type="button" variant="ghost" size="sm" onClick={() => setShowReply((v) => !v)}>
                {showReply ? 'Cancel reply' : 'Reply'}
              </Button>
            )}
          </div>

          {showReply && onReply && (
            <div className="mt-3 border-t border-slate-200 pt-3">
              <CommentForm
                parentCommentId={comment.id}
                onSubmit={async (payload) => {
                  await onReply(payload)
                  setShowReply(false)
                }}
                isSubmitting={isReplying}
                submitLabel="Post reply"
              />
            </div>
          )}
        </div>
      </div>

      {replies.length > 0 && (
        <div className="mt-3 space-y-3">
          {replies.map((reply) => (
            <CommentItem
              key={reply.id}
              comment={reply}
              depth={depth + 1}
              fileContent={fileContent}
              canResolve={canResolve}
              canApplySuggestion={canApplySuggestion}
              onResolve={onResolve}
              onReply={onReply}
              onApplySuggestion={onApplySuggestion}
              isResolving={isResolving}
              isReplying={isReplying}
              isApplying={isApplying}
            />
          ))}
        </div>
      )}
    </article>
  )
}
