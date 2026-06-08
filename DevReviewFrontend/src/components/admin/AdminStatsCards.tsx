import { Card } from '@/components/common/Card'
import type { AdminStatistics } from '@/types'

interface AdminStatsCardsProps {
  stats: AdminStatistics
}

export function AdminStatsCards({ stats }: AdminStatsCardsProps) {
  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <Card padding="sm">
        <p className="text-sm text-slate-500">Total users</p>
        <p className="mt-1 text-2xl font-bold text-slate-900">{stats.totalUsers}</p>
      </Card>
      <Card padding="sm">
        <p className="text-sm text-slate-500">Active users</p>
        <p className="mt-1 text-2xl font-bold text-slate-900">{stats.totalActiveUsers}</p>
      </Card>
      <Card padding="sm">
        <p className="text-sm text-slate-500">Mentors</p>
        <p className="mt-1 text-2xl font-bold text-slate-900">{stats.totalMentors}</p>
      </Card>
      <Card padding="sm">
        <p className="text-sm text-slate-500">Authors</p>
        <p className="mt-1 text-2xl font-bold text-slate-900">{stats.totalAuthors}</p>
      </Card>
      <Card padding="sm">
        <p className="text-sm text-slate-500">Review requests</p>
        <p className="mt-1 text-2xl font-bold text-slate-900">{stats.totalReviewRequests}</p>
      </Card>
      <Card padding="sm">
        <p className="text-sm text-slate-500">Completed reviews</p>
        <p className="mt-1 text-2xl font-bold text-slate-900">{stats.totalCompletedReviews}</p>
      </Card>
      <Card padding="sm">
        <p className="text-sm text-slate-500">Office hour bookings</p>
        <p className="mt-1 text-2xl font-bold text-slate-900">{stats.totalOfficeHourBookings}</p>
      </Card>
    </div>
  )
}
