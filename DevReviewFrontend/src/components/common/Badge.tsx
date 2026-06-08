import type { HTMLAttributes } from 'react'
import { cn } from '@/utils/cn'

type BadgeVariant = 'default' | 'success' | 'warning' | 'info' | 'neutral'

interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  variant?: BadgeVariant
}

function getBadgeClass(variant: BadgeVariant): string {
  if (variant === 'success') return 'bg-emerald-100 text-emerald-700'
  if (variant === 'warning') return 'bg-amber-100 text-amber-800'
  if (variant === 'info') return 'bg-sky-100 text-sky-700'
  if (variant === 'neutral') return 'bg-slate-100 text-slate-700'
  return 'bg-brand-100 text-brand-700'
}

export function Badge({ variant = 'default', className, children, ...props }: BadgeProps) {
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium',
        getBadgeClass(variant),
        className,
      )}
      {...props}
    >
      {children}
    </span>
  )
}
