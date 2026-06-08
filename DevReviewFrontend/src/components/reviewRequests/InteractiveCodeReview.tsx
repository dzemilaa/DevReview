import { useMemo, useState } from 'react'
import { InlineCommentForm } from '@/components/comments/InlineCommentForm'
import { CommentItem } from '@/components/comments/CommentItem'
import { LineNumberedCodeViewer } from '@/components/reviewRequests/LineNumberedCodeViewer'
import type { AddCommentPayload, CodeFile, Comment } from '@/types'

function flattenComments(comments: Comment[]): Comment[] {
  return comments.flatMap((c) => [c, ...flattenComments(c.replies ?? [])])
}

interface InteractiveCodeReviewProps {
  codeFiles: CodeFile[]
  comments: Comment[]
  canComment: boolean
  canResolve?: boolean
  canApplySuggestions?: boolean
  onAddComment: (payload: AddCommentPayload) => Promise<void>
  onResolve?: (commentId: string) => Promise<void>
  onApplySuggestion?: (commentId: string) => Promise<void>
  isSubmitting?: boolean
  isResolving?: boolean
  isApplying?: boolean
}

export function InteractiveCodeReview({
  codeFiles,
  comments,
  canComment,
  canResolve = false,
  canApplySuggestions = false,
  onAddComment,
  onResolve,
  onApplySuggestion,
  isSubmitting = false,
  isResolving = false,
  isApplying = false,
}: InteractiveCodeReviewProps) {
  const sortedFiles = useMemo(
    () => [...codeFiles].sort((a, b) => a.orderIndex - b.orderIndex),
    [codeFiles],
  )

  const [commentTarget, setCommentTarget] = useState<{
    fileId: string
    startLine: number
    endLine: number
  } | null>(null)

  const flatComments = useMemo(() => flattenComments(comments), [comments])

  const getFileComments = (fileName: string) =>
    flatComments.filter((c) => c.codeFilePath === fileName && c.startLine != null)

  if (sortedFiles.length === 0) {
    return null
  }

  return (
    <section className="space-y-6">
      <div>
        <h2 className="text-lg font-semibold text-slate-900">Code review</h2>
        <p className="mt-1 text-sm text-slate-500">
          {canComment
            ? `All ${sortedFiles.length} file${sortedFiles.length === 1 ? '' : 's'} · click a line to comment`
            : `All ${sortedFiles.length} file${sortedFiles.length === 1 ? '' : 's'} submitted with this request`}
        </p>
      </div>

      {sortedFiles.map((file) => {
        const fileComments = getFileComments(file.fileName)
        const lineMarkers = fileComments.map((c) => ({
          id: c.id,
          type: c.type,
          startLine: c.startLine!,
          endLine: c.endLine ?? c.startLine!,
        }))
        const fileCommentsForList = fileComments.filter((c) => !c.parentCommentId)
        const isCommentingThisFile = commentTarget?.fileId === file.id

        return (
          <div key={file.id} className="space-y-3">
            <LineNumberedCodeViewer
              fileName={file.fileName}
              content={file.content}
              interactive={canComment}
              selectedLine={isCommentingThisFile ? commentTarget.startLine : null}
              lineComments={lineMarkers}
              onLineClick={(line) => {
                if (!canComment) return
                setCommentTarget({ fileId: file.id, startLine: line, endLine: line })
              }}
            />

            {canComment && isCommentingThisFile && commentTarget && (
              <InlineCommentForm
                fileName={file.fileName}
                startLine={commentTarget.startLine}
                endLine={commentTarget.endLine}
                fileContent={file.content}
                isSubmitting={isSubmitting}
                onCancel={() => setCommentTarget(null)}
                onSubmit={async (payload) => {
                  await onAddComment(payload)
                  setCommentTarget(null)
                }}
              />
            )}

            {fileCommentsForList.length > 0 && (
              <div className="space-y-3 pl-1">
                <h3 className="text-sm font-semibold text-slate-700">
                  Inline comments on {file.fileName}
                </h3>
                {fileCommentsForList.map((comment) => (
                  <CommentItem
                    key={comment.id}
                    comment={comment}
                    fileContent={file.content}
                    canResolve={canResolve}
                    canApplySuggestion={canApplySuggestions && !comment.isResolved}
                    onResolve={onResolve}
                    onApplySuggestion={onApplySuggestion}
                    isResolving={isResolving}
                    isApplying={isApplying}
                  />
                ))}
              </div>
            )}
          </div>
        )
      })}
    </section>
  )
}
