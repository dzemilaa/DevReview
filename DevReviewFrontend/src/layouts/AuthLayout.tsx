import { Outlet } from 'react-router-dom'
import { Link } from 'react-router-dom'
import { APP_NAME, ROUTES } from '@/utils/constants'

export function AuthLayout() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-gradient-to-br from-slate-50 via-white to-brand-50 px-4 py-12">
      <Link to={ROUTES.home} className="mb-8 flex items-center gap-2">
        <span className="flex h-10 w-10 items-center justify-center rounded-xl bg-brand-600 text-sm font-bold text-white">
          DR
        </span>
        <span className="text-xl font-semibold text-slate-900">{APP_NAME}</span>
      </Link>

      <div className="w-full max-w-md">
        <Outlet />
      </div>
    </div>
  )
}
