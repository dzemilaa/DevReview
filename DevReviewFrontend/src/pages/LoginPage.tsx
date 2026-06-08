import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Input } from '@/components/common/Input'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/utils/constants'

export function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const { login, error, clearError, isLoading } = useAuth()
  const [usernameOrEmail, setUsernameOrEmail] = useState('')
  const [password, setPassword] = useState('')

  const from = (location.state as { from?: { pathname: string } } | null)?.from?.pathname ?? ROUTES.reviews

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    clearError()
    try {
      await login(usernameOrEmail, password)
      void navigate(from, { replace: true })
    } catch {
      // error handled in store
    }
  }

  return (
    <Card>
      <h1 className="text-2xl font-bold text-slate-900">Welcome back</h1>
      <p className="mt-1 text-sm text-slate-500">Sign in to your DevReview account</p>

      <form onSubmit={(e) => void handleSubmit(e)} className="mt-6 space-y-4">
        <Input
          label="Username or email"
          value={usernameOrEmail}
          onChange={(e) => setUsernameOrEmail(e.target.value)}
          autoComplete="username"
          required
        />
        <Input
          label="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="current-password"
          required
        />

        {error && (
          <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700" role="alert">
            {error}
          </p>
        )}

        <Button type="submit" className="w-full" isLoading={isLoading}>
          Sign in
        </Button>
      </form>

      <p className="mt-6 text-center text-sm text-slate-500">
        No account?{' '}
        <Link to={ROUTES.register} className="font-medium text-brand-600 hover:text-brand-700">
          Register
        </Link>
      </p>
    </Card>
  )
}
