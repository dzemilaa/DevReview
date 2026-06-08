import { Navigate, Outlet } from 'react-router-dom'
import { Spinner } from '@/components/common/Spinner'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/utils/constants'

export function AdminRoute() {
  const { isAuthenticated, isAdmin, isLoading } = useAuth()

  if (isLoading) {
    return <Spinner label="Checking session..." />
  }

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.login} replace />
  }

  if (!isAdmin) {
    return <Navigate to={ROUTES.home} replace />
  }

  return <Outlet />
}
