import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Textarea } from '@/components/common/Textarea'
import type { AddCommentPayload } from '@/types'
import { CommentType } from '@/types'

interface InlineCommentFormProps {
  fileName: string
  startLine: number
  endLine: number
  onSubmit: (payload: AddCommentPayload) => Promise<void>
  onCancel: () => void
  isSubmitting?: boolean
}

export function InlineCommentForm({
  fileName,
  startLine,
  endLine,
  onSubmit,
  onCancel,
  isSubmitting = false,
}: InlineCommentFormProps) {
  const [content, setContent] = useState('')
  const [commentType, setCommentType] = useState<CommentType>(CommentType.Suggestion)
  const [lineEnd, setLineEnd] = useState(String(endLine))
  const [suggestedCode, setSuggestedCode] = useState('')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!content.trim()) return

    const end = Math.max(startLine, Number(lineEnd) || startLine)
    await onSubmit({
      content: content.trim(),
      commentType,
      codeFilePath: fileName,
      startLine,
      endLine: end,
      suggestedCode: suggestedCode.trim() || undefined,
    })
  }

  return (
    <form
      onSubmit={(e) => void handleSubmit(e)}
      className="rounded-lg border border-brand-200 bg-brand-50/40 p-4 space-y-3"
    >
      <div className="flex items-center justify-between gap-2">
        <p className="text-sm font-medium text-slate-800">
          Comment on <span className="font-mono text-brand-700">{fileName}</span>
        </p>
        <span className="rounded-md bg-white px-2 py-0.5 text-xs font-medium text-slate-600">
          Lines {startLine}–{Number(lineEnd) || startLine}
        </span>
      </div>

      <div className="grid gap-3 sm:grid-cols-2">
        <div className="space-y-1.5">
          <label htmlFor="inline-end-line" className="block text-sm font-medium text-slate-700">
            End line
          </label>
          <input
            id="inline-end-line"
            type="number"
            min={startLine}
            value={lineEnd}
            onChange={(e) => setLineEnd(e.target.value)}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm"
          />
        </div>
        <div className="space-y-1.5">
          <label htmlFor="inline-comment-type" className="block text-sm font-medium text-slate-700">
            Comment type
          </label>
          <select
            id="inline-comment-type"
            value={commentType}
            onChange={(e) => setCommentType(Number(e.target.value) as CommentType)}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm"
          >
            <option value={CommentType.Suggestion}>Suggestion</option>
            <option value={CommentType.Question}>Question</option>
            <option value={CommentType.Praise}>Praise</option>
            <option value={CommentType.Critical}>Critical</option>
          </select>
        </div>
      </div>

      <Textarea
        label="Comment"
        value={content}
        onChange={(e) => setContent(e.target.value)}
        rows={3}
        placeholder="Explain your feedback..."
        required
      />

      <Textarea
        label="Suggested change (optional)"
        value={suggestedCode}
        onChange={(e) => setSuggestedCode(e.target.value)}
        rows={3}
        className="font-mono text-xs"
        placeholder="// Replacement code for the selected lines"
      />

      <div className="flex gap-2">
        <Button type="submit" size="sm" isLoading={isSubmitting} disabled={!content.trim()}>
          Post comment
        </Button>
        <Button type="button" variant="ghost" size="sm" onClick={onCancel}>
          Cancel
        </Button>
      </div>
    </form>
  )
}
