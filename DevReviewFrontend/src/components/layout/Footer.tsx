import { APP_NAME } from '@/utils/constants'

export function Footer() {
  return (
    <footer className="border-t border-slate-200 bg-white">
      <div className="mx-auto flex max-w-6xl flex-col items-center justify-between gap-2 px-4 py-6 text-sm text-slate-500 sm:flex-row sm:px-6">
        <p>&copy; {new Date().getFullYear()} {APP_NAME}</p>
      </div>
    </footer>
  )
}
