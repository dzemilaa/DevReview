import { useState } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { Button } from '@/components/common/Button'
import { NotificationBell } from '@/components/notifications/NotificationBell'
import { UserAvatar } from '@/components/users/UserAvatar'
import { useAuth } from '@/hooks/useAuth'
import { APP_NAME, ROUTES } from '@/utils/constants'
import { cn } from '@/utils/cn'

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    'rounded-lg px-3 py-2 text-sm font-medium transition-colors',
    isActive ? 'bg-brand-50 text-brand-700' : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900',
  )

const mobileNavLinkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    'block rounded-lg px-3 py-2.5 text-base font-medium transition-colors',
    isActive ? 'bg-brand-50 text-brand-700' : 'text-slate-700 hover:bg-slate-100',
  )

export function Header() {
  const { isAuthenticated, user, logout, isAdmin, isMentor } = useAuth()
  const [menuOpen, setMenuOpen] = useState(false)
  const close = () => setMenuOpen(false)

  return (
    <header className="sticky top-0 z-40 border-b border-slate-200 bg-white">
      <div className="mx-auto flex h-16 max-w-6xl items-center justify-between gap-4 px-4 sm:px-6">
        {/* Logo */}
        <div className="flex min-w-0 items-center gap-4 lg:gap-8">
          <Link to={ROUTES.home} className="flex shrink-0 items-center gap-2" onClick={close}>
            <span className="flex h-8 w-8 items-center justify-center rounded-lg bg-brand-600 text-sm font-bold text-white">
              DR
            </span>
            <span className="hidden text-lg font-semibold text-slate-900 sm:inline">{APP_NAME}</span>
          </Link>

          {/* Desktop nav */}
          <nav className="hidden items-center gap-0.5 md:flex">
            <NavLink to={ROUTES.knowledgeBase} className={navLinkClass}>Knowledge base</NavLink>
            <NavLink to={ROUTES.mentors} className={navLinkClass}>Mentors</NavLink>
            <NavLink to={ROUTES.officeHours} className={navLinkClass}>Office hours</NavLink>
            {isAuthenticated && (
              <>
                <NavLink to={ROUTES.reviews} className={navLinkClass}>Reviews</NavLink>
                <NavLink to={ROUTES.myReviews} className={navLinkClass}>My requests</NavLink>
                {isMentor && (
                  <NavLink to={ROUTES.officeHoursSchedule} className={navLinkClass}>Schedule</NavLink>
                )}
                {isAdmin && (
                  <NavLink to={ROUTES.admin} className={navLinkClass}>Admin</NavLink>
                )}
              </>
            )}
          </nav>
        </div>

        {/* Desktop right side */}
        <div className="hidden items-center gap-2 sm:gap-3 md:flex">
          {isAuthenticated ? (
            <>
              <NotificationBell />
              <Link
                to={ROUTES.profile}
                className="hidden items-center gap-2 rounded-lg px-2 py-1 hover:bg-slate-100 sm:flex"
              >
                <UserAvatar name={user?.displayName ?? user?.userName ?? 'User'} />
                <span className="max-w-30 truncate text-sm font-medium text-slate-700">
                  {user?.displayName ?? user?.userName}
                </span>
              </Link>
              <Button variant="ghost" size="sm" onClick={() => void logout()}>
                Sign out
              </Button>
            </>
          ) : (
            <>
              <Link to={ROUTES.login}>
                <Button variant="ghost" size="sm">Sign in</Button>
              </Link>
              <Link to={ROUTES.register}>
                <Button size="sm">Get started</Button>
              </Link>
            </>
          )}
        </div>

        {/* Mobile right: bell + hamburger */}
        <div className="flex items-center gap-2 md:hidden">
          {isAuthenticated && <NotificationBell />}
          <button
            type="button"
            aria-label="Toggle menu"
            onClick={() => setMenuOpen((o) => !o)}
            className="rounded-lg p-2 text-slate-600 hover:bg-slate-100"
          >
            {menuOpen ? (
              <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            ) : (
              <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            )}
          </button>
        </div>
      </div>

      {/* Mobile menu */}
      {menuOpen && (
        <div className="border-t border-slate-200 bg-white px-4 pb-4 md:hidden">
          <nav className="mt-2 space-y-1">
            <NavLink to={ROUTES.knowledgeBase} className={mobileNavLinkClass} onClick={close}>Knowledge base</NavLink>
            <NavLink to={ROUTES.mentors} className={mobileNavLinkClass} onClick={close}>Mentors</NavLink>
            <NavLink to={ROUTES.officeHours} className={mobileNavLinkClass} onClick={close}>Office hours</NavLink>
            {isAuthenticated && (
              <>
                <NavLink to={ROUTES.reviews} className={mobileNavLinkClass} onClick={close}>Reviews</NavLink>
                <NavLink to={ROUTES.myReviews} className={mobileNavLinkClass} onClick={close}>My requests</NavLink>
                {isMentor && (
                  <NavLink to={ROUTES.officeHoursSchedule} className={mobileNavLinkClass} onClick={close}>Schedule</NavLink>
                )}
                {isAdmin && (
                  <NavLink to={ROUTES.admin} className={mobileNavLinkClass} onClick={close}>Admin</NavLink>
                )}
              </>
            )}
          </nav>

          <div className="mt-4 border-t border-slate-100 pt-4">
            {isAuthenticated ? (
              <div className="space-y-2">
                <Link
                  to={ROUTES.profile}
                  onClick={close}
                  className="flex items-center gap-3 rounded-lg px-3 py-2 hover:bg-slate-100"
                >
                  <UserAvatar name={user?.displayName ?? user?.userName ?? 'User'} />
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium text-slate-900">
                      {user?.displayName ?? user?.userName}
                    </p>
                    <p className="truncate text-xs text-slate-500">{user?.email}</p>
                  </div>
                </Link>
                <Button
                  variant="ghost"
                  size="sm"
                  className="w-full justify-start"
                  onClick={() => { void logout(); close() }}
                >
                  Sign out
                </Button>
              </div>
            ) : (
              <div className="flex flex-col gap-2">
                <Link to={ROUTES.login} onClick={close}>
                  <Button variant="secondary" size="sm" className="w-full">Sign in</Button>
                </Link>
                <Link to={ROUTES.register} onClick={close}>
                  <Button size="sm" className="w-full">Get started</Button>
                </Link>
              </div>
            )}
          </div>
        </div>
      )}
    </header>
  )
}
