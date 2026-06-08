import { cn } from '@/utils/cn'

interface SpinnerProps {
  className?: string
  label?: string
}

export function Spinner({ className, label = 'Loading...' }: SpinnerProps) {
  return (
    <div className={cn('flex flex-col items-center justify-center gap-3 py-12', className)}>
      <div
        className="h-8 w-8 animate-spin rounded-full border-2 border-brand-600 border-t-transparent"
        role="status"
        aria-label={label}
      />
      <p className="text-sm text-slate-500">{label}</p>
    </div>
  )
}
