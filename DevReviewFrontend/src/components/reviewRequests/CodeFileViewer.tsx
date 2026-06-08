import { Card } from '@/components/common/Card'

interface CodeFileViewerProps {
  fileName: string
  content: string
}

export function CodeFileViewer({ fileName, content }: CodeFileViewerProps) {
  const lineCount = content.split('\n').length

  return (
    <Card padding="none" className="overflow-hidden">
      <div className="flex items-center justify-between border-b border-slate-200 bg-slate-900 px-4 py-2">
        <span className="text-sm font-medium text-slate-200">{fileName}</span>
        <span className="rounded bg-slate-700 px-1.5 py-0.5 text-xs text-slate-400">
          {lineCount} {lineCount === 1 ? 'line' : 'lines'}
        </span>
      </div>
      <pre className="overflow-x-auto bg-slate-950 p-4 text-xs leading-relaxed text-slate-200">
        <code>{content}</code>
      </pre>
    </Card>
  )
}
