import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Textarea } from '@/components/common/Textarea'
import type { AddCommentPayload } from '@/types'
import { CommentType } from '@/types'

interface CommentFormProps {
  parentCommentId?: string
  onSubmit: (payload: AddCommentPayload) => Promise<void>
  isSubmitting?: boolean
  submitLabel?: string
}

export function CommentForm({
  parentCommentId,
  onSubmit,
  isSubmitting = false,
  submitLabel = 'Post comment',
}: CommentFormProps) {
  const [content, setContent] = useState('')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!content.trim()) return
    await onSubmit({ content: content.trim(), commentType: CommentType.General, parentCommentId })
    setContent('')
  }

  return (
    <form onSubmit={(e) => void handleSubmit(e)} className="space-y-3">
      <Textarea
        label={parentCommentId ? 'Reply' : 'Add a comment'}
        value={content}
        onChange={(e) => setContent(e.target.value)}
        placeholder="Write a comment..."
        rows={parentCommentId ? 3 : 4}
        required
      />

      <Button type="submit" isLoading={isSubmitting} disabled={!content.trim()}>
        {submitLabel}
      </Button>
    </form>
  )
}
