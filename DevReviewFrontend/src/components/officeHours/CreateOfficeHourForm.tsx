import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Input } from '@/components/common/Input'
import type { CreateOfficeHourPayload } from '@/types'

interface CreateOfficeHourFormProps {
  onSubmit: (payload: CreateOfficeHourPayload) => Promise<void>
  isSubmitting?: boolean
}

function toLocalDatetimeValue(date: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`
}

export function CreateOfficeHourForm({ onSubmit, isSubmitting = false }: CreateOfficeHourFormProps) {
  const defaultStart = new Date()
  defaultStart.setHours(defaultStart.getHours() + 1, 0, 0, 0)

  const [startTime, setStartTime] = useState(toLocalDatetimeValue(defaultStart))
  const [durationMinutes, setDurationMinutes] = useState('60')
  const [topic, setTopic] = useState('')
  const [price, setPrice] = useState('')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    await onSubmit({
      startTime: new Date(startTime).toISOString(),
      durationMinutes: Number(durationMinutes),
      topic,
      price: price ? Number(price) : null,
    })
    setTopic('')
    setPrice('')
  }

  return (
    <Card>
      <h2 className="text-lg font-semibold text-slate-900">Create office hour slot</h2>
      <form onSubmit={(e) => void handleSubmit(e)} className="mt-4 space-y-4">
        <Input
          label="Topic"
          value={topic}
          onChange={(e) => setTopic(e.target.value)}
          placeholder="e.g. React performance review"
          required
        />
        <Input
          label="Start time"
          type="datetime-local"
          value={startTime}
          onChange={(e) => setStartTime(e.target.value)}
          required
        />
        <Input
          label="Duration (minutes)"
          type="number"
          min={15}
          step={15}
          value={durationMinutes}
          onChange={(e) => setDurationMinutes(e.target.value)}
          required
        />
        <Input
          label="Price (optional)"
          type="number"
          min={0}
          step={0.01}
          value={price}
          onChange={(e) => setPrice(e.target.value)}
          placeholder="0.00"
        />
        <Button type="submit" isLoading={isSubmitting}>
          Create slot
        </Button>
      </form>
    </Card>
  )
}
