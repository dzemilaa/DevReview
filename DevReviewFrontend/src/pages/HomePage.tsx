import { Link } from 'react-router-dom'
import { Button } from '@/components/common/Button'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/utils/constants'

export function HomePage() {
  const { isAuthenticated } = useAuth()

  return (
    <div className="space-y-12 mt-45">
      <section className="text-center">
        <p className="text-sm font-semibold uppercase tracking-wide text-brand-600">
          Code Review Platform
        </p>
        <h1 className="mt-3 text-4xl font-bold tracking-tight text-slate-900 sm:text-5xl">
         Get expert feedback on your code
        </h1>
        <p className="mx-auto mt-4 max-w-2xl text-lg text-slate-600">
          Submit your code and receive structured reviews with inline comments
          from other developers.
        </p>
        <div className="mt-8 flex flex-wrap items-center justify-center gap-3">
          {isAuthenticated ? (
            <>
              <Link to={ROUTES.reviews}>
                <Button size="lg">Browse reviews</Button>
              </Link>
              <Link to={ROUTES.newReview}>
                <Button variant="secondary" size="lg">
                  New request
                </Button>
              </Link>
            </>
          ) : (
            <>
              <Link to={ROUTES.register}>
                <Button size="lg">Create account</Button>
              </Link>
              <Link to={ROUTES.login}>
                <Button variant="secondary" size="lg">
                  Sign in
                </Button>
              </Link>
            </>
          )}
        </div>
      </section>
    </div>
  )
}