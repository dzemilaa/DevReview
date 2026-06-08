import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Input } from '@/components/common/Input'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/utils/constants'

export function RegisterPage() {
  const navigate = useNavigate()
  const { register, error, clearError, isLoading } = useAuth()
  const [userName, setUserName] = useState('')
  const [email, setEmail] = useState('')
  const [displayName, setDisplayName] = useState('')
  const [password, setPassword] = useState('')
  const [role, setRole] = useState('Author')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    clearError()
    try {
      await register({ userName, email, displayName, password, role })
      void navigate(ROUTES.reviews, { replace: true })
    } catch {
      // error handled in store
    }
  }

  return (
    <Card>
      <h1 className="text-2xl font-bold text-slate-900">Create account</h1>
      <p className="mt-1 text-sm text-slate-500">Join DevReview and start getting feedback</p>

      <form onSubmit={(e) => void handleSubmit(e)} className="mt-6 space-y-4">
        <Input label="Username" value={userName} onChange={(e) => setUserName(e.target.value)} required />
        <Input
          label="Display name"
          value={displayName}
          onChange={(e) => setDisplayName(e.target.value)}
          required
        />
        <Input
          label="Email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          autoComplete="email"
          required
        />
        <Input
          label="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="new-password"
          required
        />

        <div className="space-y-1.5">
          <label htmlFor="role" className="block text-sm font-medium text-slate-700">
            I want to be a
          </label>
          <select
            id="role"
            value={role}
            onChange={(e) => setRole(e.target.value)}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/20"
          >
            <option value="Author">Author</option>
            <option value="Mentor">Mentor</option>
          </select>
        </div>

        {error && (
          <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700" role="alert">
            {error}
          </p>
        )}

        <Button type="submit" className="w-full" isLoading={isLoading}>
          Register
        </Button>
      </form>

      <p className="mt-6 text-center text-sm text-slate-500">
        Already have an account?{' '}
        <Link to={ROUTES.login} className="font-medium text-brand-600 hover:text-brand-700">
          Sign in
        </Link>
      </p>
    </Card>
  )
}
