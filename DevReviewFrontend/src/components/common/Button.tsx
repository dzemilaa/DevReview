import type { ButtonHTMLAttributes } from 'react'
import { cn } from '@/utils/cn'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  isLoading?: boolean
}

function getVariantClass(variant: string): string {
  if (variant === 'secondary') return 'border border-slate-300 bg-white text-slate-700 hover:bg-slate-50'
  if (variant === 'ghost') return 'text-slate-600 hover:bg-slate-100'
  if (variant === 'danger') return 'bg-red-600 text-white hover:bg-red-700'
  return 'bg-brand-600 text-white hover:bg-brand-700 disabled:bg-brand-300'
}

function getSizeClass(size: string): string {
  if (size === 'sm') return 'px-3 py-1.5 text-sm'
  if (size === 'lg') return 'px-5 py-2.5 text-base'
  return 'px-4 py-2 text-sm'
}

export function Button({
  variant = 'primary',
  size = 'md',
  isLoading = false,
  className,
  children,
  disabled,
  ...props
}: ButtonProps) {
  return (
    <button
      type="button"
      className={cn(
        'inline-flex items-center justify-center gap-2 rounded-lg font-medium transition-colors disabled:cursor-not-allowed disabled:opacity-60',
        getVariantClass(variant),
        getSizeClass(size),
        className,
      )}
      disabled={disabled || isLoading}
      {...props}
    >
      {isLoading && (
        <span className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
      )}
      {children}
    </button>
  )
}
