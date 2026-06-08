import { CommentItem } from './CommentItem'
import { EmptyState } from '@/components/common/EmptyState'
import type { AddCommentPayload, Comment } from '@/types'

interface CommentListProps {
  comments: Comment[]
  canResolve?: boolean
  onResolve?: (commentId: string) => Promise<void>
  onReply?: (payload: AddCommentPayload) => Promise<void>
  isResolving?: boolean
  isReplying?: boolean
}

export function CommentList({
  comments,
  canResolve = false,
  onResolve,
  onReply,
  isResolving = false,
  isReplying = false,
}: CommentListProps) {
  if (comments.length === 0) {
    return (
      <EmptyState
        title="No comments yet"
      />
    )
  }

  return (
    <div className="space-y-3">
      {comments.map((comment) => (
        <CommentItem
          key={comment.id}
          comment={comment}
          canResolve={canResolve}
          onResolve={onResolve}
          onReply={onReply}
          isResolving={isResolving}
          isReplying={isReplying}
        />
      ))}
    </div>
  )
}
