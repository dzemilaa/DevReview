import { Navigate, Outlet } from 'react-router-dom'
import { Spinner } from '@/components/common/Spinner'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/utils/constants'

export function MentorRoute() {
  const { isAuthenticated, isMentor, isLoading } = useAuth()

  if (isLoading) {
    return <Spinner label="Checking session..." />
  }

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.login} replace />
  }

  if (!isMentor) {
    return <Navigate to={ROUTES.officeHours} replace />
  }

  return <Outlet />
}
