import { cn } from '@/utils/cn'

interface UserAvatarProps {
  name: string
  size?: 'sm' | 'md' | 'lg'
  className?: string
}

function getInitials(name: string): string {
  return name
    .split(' ')
    .map((part) => part[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

export function UserAvatar({ name, size = 'sm', className }: UserAvatarProps) {
  const sizeClass = size === 'lg' ? 'h-12 w-12 text-base' : size === 'md' ? 'h-10 w-10 text-sm' : 'h-8 w-8 text-xs'

  return (
    <div
      className={cn(
        'flex shrink-0 items-center justify-center rounded-full bg-brand-100 font-semibold text-brand-700',
        sizeClass,
        className,
      )}
      title={name}
    >
      {getInitials(name)}
    </div>
  )
}
