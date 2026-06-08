import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Textarea } from '@/components/common/Textarea'

interface FinalizeReviewFormProps {
  onSubmit: (payload: {
    summary: string
    priorityFixes: string
    qualityScore: number
  }) => Promise<void>
  isSubmitting?: boolean
}

export function FinalizeReviewForm({ onSubmit, isSubmitting = false }: FinalizeReviewFormProps) {
  const [summary, setSummary] = useState('')
  const [priorityFixes, setPriorityFixes] = useState('')
  const [qualityScore, setQualityScore] = useState(4)

  return (
    <Card className="border-emerald-200 bg-emerald-50/30">
      <h3 className="font-semibold text-slate-900">Submit final review</h3>
      <p className="mt-1 text-sm text-slate-600">
        Provide a summary, quality score, and priority fixes for the author.
      </p>
      <form
        onSubmit={(e) => {
          e.preventDefault()
          void onSubmit({
            summary: summary.trim(),
            priorityFixes: priorityFixes.trim(),
            qualityScore,
          })
        }}
        className="mt-4 space-y-4"
      >
        <Textarea
          label="Summary"
          value={summary}
          onChange={(e) => setSummary(e.target.value)}
          rows={4}
          placeholder="Overall assessment of the codebase..."
          required
        />
        <Textarea
          label="Priority fixes"
          value={priorityFixes}
          onChange={(e) => setPriorityFixes(e.target.value)}
          rows={4}
          placeholder="List the most important changes the author should make first..."
          required
        />
        <div className="space-y-1.5">
          <label htmlFor="quality-score" className="block text-sm font-medium text-slate-700">
            Quality score (1–5)
          </label>
          <select
            id="quality-score"
            value={qualityScore}
            onChange={(e) => setQualityScore(Number(e.target.value))}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm"
          >
            {[1, 2, 3, 4, 5].map((n) => (
              <option key={n} value={n}>{n}</option>
            ))}
          </select>
        </div>

        <Button
          type="submit"
          isLoading={isSubmitting}
          disabled={!summary.trim() || !priorityFixes.trim()}
        >
          Complete review
        </Button>
      </form>
    </Card>
  )
}
