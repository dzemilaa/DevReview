import { useState } from 'react'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Input } from '@/components/common/Input'
import { Textarea } from '@/components/common/Textarea'
import { CodeFilesEditor } from './CodeFilesEditor'
import type { CodeFileInput, CreateReviewRequestPayload } from '@/types'
import { DifficultyLevel } from '@/types'

interface ReviewRequestFormProps {
  onSubmit: (payload: CreateReviewRequestPayload) => Promise<void>
  isSubmitting?: boolean
}

export function ReviewRequestForm({ onSubmit, isSubmitting = false }: ReviewRequestFormProps) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [programmingLanguage, setProgrammingLanguage] = useState('')
  const [framework, setFramework] = useState('')
  const [difficulty, setDifficulty] = useState<DifficultyLevel>(DifficultyLevel.Junior)
  const [tags, setTags] = useState('')
  const [isPublic, setIsPublic] = useState(true)
  const [isPaid, setIsPaid] = useState(false)
  const [price, setPrice] = useState('0')
  const [codeFiles, setCodeFiles] = useState<CodeFileInput[]>([])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    const parsedTags = tags
      .split(',')
      .map((t) => t.trim())
      .filter(Boolean)

    const filesWithContent = codeFiles.filter((f) => f.content.trim())

    await onSubmit({
      title,
      description,
      programmingLanguage,
      framework,
      difficulty,
      isPublic,
      isPaid,
      price: isPaid ? Number(price) : 0,
      tags: parsedTags,
      codeFiles: filesWithContent.map((f, i) => ({ ...f, orderIndex: i })),
    })
  }

  return (
    <Card>
      <form onSubmit={(e) => void handleSubmit(e)} className="space-y-5">
        <Input label="Title" value={title} onChange={(e) => setTitle(e.target.value)} required />
        <Textarea
          label="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          rows={4}
          placeholder="Describe what this code does and what you want reviewed (e.g. performance, code cleanliness, naming, architecture...)"
          required
        />
        <div className="grid gap-4 sm:grid-cols-2">
          <Input
            label="Programming language"
            value={programmingLanguage}
            onChange={(e) => setProgrammingLanguage(e.target.value)}
            placeholder="TypeScript"
            required
          />
          <Input
            label="Framework"
            value={framework}
            onChange={(e) => setFramework(e.target.value)}
            placeholder="React"
          />
        </div>

        <div className="space-y-1.5">
          <label htmlFor="difficulty" className="block text-sm font-medium text-slate-700">
            Difficulty
          </label>
          <select
            id="difficulty"
            value={difficulty}
            onChange={(e) => setDifficulty(Number(e.target.value) as DifficultyLevel)}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm"
          >
            <option value={DifficultyLevel.Junior}>Junior</option>
            <option value={DifficultyLevel.Mid}>Mid</option>
            <option value={DifficultyLevel.Senior}>Senior</option>
          </select>
        </div>

        <Input
          label="Tags (comma-separated)"
          value={tags}
          onChange={(e) => setTags(e.target.value)}
          placeholder="api, refactor, backend"
        />

        <div className="flex flex-wrap gap-6">
          <label className="flex items-center gap-2 text-sm text-slate-700">
            <input
              type="checkbox"
              checked={isPublic}
              onChange={(e) => setIsPublic(e.target.checked)}
              className="h-4 w-4 rounded border-slate-300"
            />
            Public request
          </label>
          <label className="flex items-center gap-2 text-sm text-slate-700">
            <input
              type="checkbox"
              checked={isPaid}
              onChange={(e) => setIsPaid(e.target.checked)}
              className="h-4 w-4 rounded border-slate-300"
            />
            Paid review
          </label>
        </div>

        {isPaid && (
          <Input
            label="Price"
            type="number"
            min={0}
            step={0.01}
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            required
          />
        )}

        <CodeFilesEditor files={codeFiles} onChange={setCodeFiles} />

        <div className="flex justify-end">
          <Button type="submit" isLoading={isSubmitting}>
            Submit review request
          </Button>
        </div>
      </form>
    </Card>
  )
}
