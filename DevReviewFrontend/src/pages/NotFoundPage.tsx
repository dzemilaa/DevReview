import { Link } from 'react-router-dom'
import { Button } from '@/components/common/Button'
import { ROUTES } from '@/utils/constants'

export function NotFoundPage() {
  return (
    <div className="flex flex-col items-center justify-center py-20 text-center">
      <p className="text-6xl font-bold text-brand-600">404</p>
      <h1 className="mt-4 text-2xl font-bold text-slate-900">Page not found</h1>
      <p className="mt-2 text-slate-500">The page you are looking for does not exist.</p>
      <Link to={ROUTES.home} className="mt-8">
        <Button>Back to home</Button>
      </Link>
    </div>
  )
}
