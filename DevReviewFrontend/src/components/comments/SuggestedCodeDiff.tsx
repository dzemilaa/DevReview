import { Button } from '@/components/common/Button'
import { buildLineDiff } from '@/utils/codeDiff'

interface SuggestedCodeDiffProps {
  fileContent: string
  startLine: number
  endLine: number
  suggestedCode: string
  canApply?: boolean
  isApplying?: boolean
  onApply?: () => Promise<void>
}

export function SuggestedCodeDiff({
  fileContent,
  startLine,
  endLine,
  suggestedCode,
  canApply = false,
  isApplying = false,
  onApply,
}: SuggestedCodeDiffProps) {
  const diff = buildLineDiff(fileContent, startLine, endLine, suggestedCode)

  return (
    <div className="mt-3 space-y-2">
      <div className="flex items-center justify-between gap-2">
        <p className="text-xs font-semibold text-slate-500 uppercase">Proposed change</p>
        {canApply && onApply && (
          <Button type="button" size="sm" variant="secondary" isLoading={isApplying} onClick={() => void onApply()}>
            Apply
          </Button>
        )}
      </div>
      <pre className="overflow-x-auto rounded-md border border-slate-200 bg-slate-950 p-3 text-xs leading-relaxed">
        {diff.map((row, idx) => {
          let rowClass = 'flex gap-3 font-mono text-slate-500'
          if (row.type === 'remove') rowClass = 'flex gap-3 font-mono bg-red-950/60 text-red-300'
          if (row.type === 'add') rowClass = 'flex gap-3 font-mono bg-emerald-950/60 text-emerald-300'

          return (
            <div key={idx} className={rowClass}>
              <span className="w-8 shrink-0 select-none text-right opacity-60">
                {row.type === 'remove' ? '-' : row.type === 'add' ? '+' : ' '}
              </span>
              <span className="w-8 shrink-0 select-none text-right opacity-60">
                {row.lineNumber ?? ''}
              </span>
              <code className="flex-1 whitespace-pre">{row.content || ' '}</code>
            </div>
          )
        })}
      </pre>
    </div>
  )
}
