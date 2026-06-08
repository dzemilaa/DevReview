import { useMemo } from 'react'
import { cn } from '@/utils/cn'
import { lineHighlightColor } from '@/utils/codeDiff'

export interface LineCommentMarker {
  id: string
  type: number
  startLine: number
  endLine: number
}

interface LineNumberedCodeViewerProps {
  fileName: string
  content: string
  interactive?: boolean
  selectedLine?: number | null
  lineComments?: LineCommentMarker[]
  onLineClick?: (line: number) => void
  className?: string
}

export function LineNumberedCodeViewer({
  fileName,
  content,
  interactive = false,
  selectedLine = null,
  lineComments = [],
  onLineClick,
  className,
}: LineNumberedCodeViewerProps) {
  const lines = content.split('\n')

  const commentsByLine = useMemo(() => {
    const map = new Map<number, LineCommentMarker[]>()
    for (const c of lineComments) {
      const end = c.endLine ?? c.startLine
      for (let ln = c.startLine; ln <= end; ln++) {
        const existing = map.get(ln) ?? []
        existing.push(c)
        map.set(ln, existing)
      }
    }
    return map
  }, [lineComments])

  return (
    <div className={cn('rounded-xl border border-slate-200 bg-white shadow-sm', className)}>
      <div className="flex items-center justify-between border-b border-slate-200 bg-slate-900 px-4 py-2">
        <span className="text-sm font-medium text-slate-200">{fileName}</span>
        {interactive && (
          <span className="text-xs text-slate-400">Click a line to comment</span>
        )}
      </div>

      <pre className="m-0 overflow-x-auto bg-slate-950 p-0 text-xs leading-relaxed">
        <code className="block font-mono">
          {lines.map((line, idx) => {
            const lineNumber = idx + 1
            const markers = commentsByLine.get(lineNumber) ?? []
            const isSelected = selectedLine === lineNumber
            const highlight = markers[0]

            return (
              <div
                key={lineNumber}
                className={cn(
                  'flex min-h-5.5',
                  interactive && 'cursor-pointer hover:bg-slate-900/80',
                  isSelected && 'bg-brand-500/20',
                  highlight && lineHighlightColor(highlight.type),
                )}
                onClick={interactive ? () => onLineClick?.(lineNumber) : undefined}
              >
                <span className="w-12 shrink-0 select-none border-r border-slate-800 py-0.5 pr-3 text-right text-slate-500">
                  {lineNumber}
                </span>
                <span className="relative min-w-0 flex-1 whitespace-pre px-4 py-0.5 text-slate-200">
                  {line || ' '}
                  {markers.length > 0 && (
                    <span className="absolute right-2 top-1/2 -translate-y-1/2 rounded-full bg-brand-500 px-1.5 py-0.5 text-[10px] font-semibold text-white">
                      {markers.length}
                    </span>
                  )}
                </span>
              </div>
            )
          })}
        </code>
      </pre>
    </div>
  )
}
