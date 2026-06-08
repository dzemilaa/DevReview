import { useRef, useState } from 'react'
import { reviewRequestsApi } from '@/api'
import { Button } from '@/components/common/Button'
import { Input } from '@/components/common/Input'
import { Textarea } from '@/components/common/Textarea'
import type { CodeFileInput } from '@/types'

interface CodeFilesEditorProps {
  files: CodeFileInput[]
  onChange: (files: CodeFileInput[]) => void
}

const emptyFile = (): CodeFileInput => ({
  fileName: 'main.ts',
  content: '',
  orderIndex: 0,
})

export function CodeFilesEditor({ files, onChange }: CodeFilesEditorProps) {
  const zipInputRef = useRef<HTMLInputElement>(null)
  const [zipError, setZipError] = useState<string | null>(null)
  const [isParsingZip, setIsParsingZip] = useState(false)

  const updateFile = (index: number, partial: Partial<CodeFileInput>) => {
    const next = files.map((f, i) => (i === index ? { ...f, ...partial, orderIndex: i } : { ...f, orderIndex: i }))
    onChange(next)
  }

  const addFile = () => {
    onChange([...files, { ...emptyFile(), orderIndex: files.length }])
  }

  const removeFile = (index: number) => {
    onChange(files.filter((_, i) => i !== index).map((f, i) => ({ ...f, orderIndex: i })))
  }

  const handleZipUpload = async (file: File) => {
    setZipError(null)
    setIsParsingZip(true)
    try {
      const parsed = await reviewRequestsApi.parseZip(file)
      onChange(parsed.map((f, i) => ({ ...f, orderIndex: i })))
    } catch {
      setZipError('Could not parse ZIP. Use a .zip under 5 MB with supported source files.')
    } finally {
      setIsParsingZip(false)
    }
  }

  return (
    <div className="space-y-4 rounded-lg border border-slate-200 bg-slate-50 p-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h3 className="text-sm font-semibold text-slate-800">Code files (optional)</h3>
        <div className="flex flex-wrap gap-2">
          <input
            ref={zipInputRef}
            type="file"
            accept=".zip"
            className="hidden"
            onChange={(e) => {
              const file = e.target.files?.[0]
              if (file) void handleZipUpload(file)
              e.target.value = ''
            }}
          />
          <Button
            type="button"
            variant="secondary"
            size="sm"
            isLoading={isParsingZip}
            onClick={() => zipInputRef.current?.click()}
          >
            Upload ZIP
          </Button>
          <Button type="button" variant="secondary" size="sm" onClick={addFile}>
            Add file
          </Button>
        </div>
      </div>

      {zipError && <p className="text-sm text-red-600">{zipError}</p>}

      {files.map((file, index) => (
        <div key={index} className="space-y-3 rounded-lg border border-slate-200 bg-white p-3">
          <div className="flex items-start justify-between gap-2">
            <Input
              label="File name"
              value={file.fileName}
              onChange={(e) => updateFile(index, { fileName: e.target.value })}
            />
            <Button type="button" variant="ghost" size="sm" onClick={() => removeFile(index)}>
              Remove
            </Button>
          </div>
          <Textarea
            label="Content"
            value={file.content}
            onChange={(e) => updateFile(index, { content: e.target.value })}
            rows={20}
            className="font-mono text-xs"
            placeholder="// Paste code"
          />
        </div>
      ))}
    </div>
  )
}