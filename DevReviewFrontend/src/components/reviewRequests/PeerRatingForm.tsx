import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'

interface PeerRatingFormProps {
  label: string
  onSubmit: (rating: number) => Promise<void>
  isSubmitting?: boolean
}

export function PeerRatingForm({ label, onSubmit, isSubmitting = false }: PeerRatingFormProps) {
  const [rating, setRating] = useState(5)
  const [submitted, setSubmitted] = useState(false)

  if (submitted) {
    return (
      <Card className="border-emerald-200 bg-emerald-50/40">
        <p className="text-sm font-medium text-emerald-800">{label}</p>
        <p className="mt-2 text-sm text-slate-600">Rating submitted. Thank you!</p>
      </Card>
    )
  }

  return (
    <Card>
      <form
        onSubmit={(e) => {
          e.preventDefault()
          void onSubmit(rating).then(() => setSubmitted(true))
        }}
        className="space-y-3"
      >
        <p className="text-sm font-medium text-slate-800">{label}</p>
        <div className="flex items-center gap-2">
          <label htmlFor="peer-rating" className="text-sm text-slate-600">
            Rating (1–5)
          </label>
          <input
            id="peer-rating"
            type="number"
            min={1}
            max={5}
            value={rating}
            onChange={(e) => setRating(Number(e.target.value))}
            className="w-16 rounded-lg border border-slate-300 px-2 py-1 text-sm"
          />
        </div>
        <Button type="submit" size="sm" isLoading={isSubmitting}>
          Submit rating
        </Button>
      </form>
    </Card>
  )
}
