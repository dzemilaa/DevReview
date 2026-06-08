import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Textarea } from '@/components/common/Textarea'

interface AbandonReviewFormProps {
  onSubmit: (reason: string) => Promise<void>
  isSubmitting?: boolean
}

export function AbandonReviewForm({ onSubmit, isSubmitting = false }: AbandonReviewFormProps) {
  const [reason, setReason] = useState('')

  return (
    <Card className="border-amber-200 bg-amber-50/50">
      <h3 className="font-semibold text-slate-900">Abandon review</h3>
      <p className="mt-1 text-sm text-slate-600">
        Release this request back to the queue. A reason is required.
      </p>
      <form
        onSubmit={(e) => {
          e.preventDefault()
          void onSubmit(reason.trim())
        }}
        className="mt-4 space-y-3"
      >
        <Textarea
          label="Reason"
          value={reason}
          onChange={(e) => setReason(e.target.value)}
          rows={3}
          required
        />
        <Button type="submit" variant="danger" isLoading={isSubmitting} disabled={!reason.trim()}>
          Abandon request
        </Button>
      </form>
    </Card>
  )
}
