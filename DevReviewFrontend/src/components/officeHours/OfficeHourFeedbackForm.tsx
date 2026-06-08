import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Textarea } from '@/components/common/Textarea'

interface OfficeHourFeedbackFormProps {
  onSubmit: (impression: string) => Promise<void>
  isSubmitting?: boolean
  existingImpression?: string | null
}

export function OfficeHourFeedbackForm({
  onSubmit,
  isSubmitting = false,
  existingImpression,
}: OfficeHourFeedbackFormProps) {
  const [impression, setImpression] = useState('')

  if (existingImpression) {
    return (
      <p className="rounded-lg bg-slate-50 px-3 py-2 text-sm text-slate-600">
        Your feedback: {existingImpression}
      </p>
    )
  }

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault()
        void onSubmit(impression.trim())
      }}
      className="space-y-2"
    >
      <Textarea
        label="Session feedback"
        value={impression}
        onChange={(e) => setImpression(e.target.value)}
        rows={3}
        placeholder="How did the session go?"
        required
      />
      <Button type="submit" size="sm" isLoading={isSubmitting} disabled={!impression.trim()}>
        Submit feedback
      </Button>
    </form>
  )
}
