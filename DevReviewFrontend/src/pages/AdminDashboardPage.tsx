import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { adminApi, reviewRequestsApi } from '@/api'
import { Spinner } from '@/components/common/Spinner'
import { AdminStatsCards } from '@/components/admin/AdminStatsCards'
import { AdminUserTable } from '@/components/admin/AdminUserTable'
import { AdminTagManager } from '@/components/admin/AdminTagManager'
import { formatDate, formatReviewStatus } from '@/utils/format'

export function AdminDashboardPage() {
  const queryClient = useQueryClient()

  const statsQuery = useQuery({
    queryKey: ['admin', 'statistics'],
    queryFn: adminApi.getStatistics,
  })

  const usersQuery = useQuery({
    queryKey: ['admin', 'users'],
    queryFn: adminApi.getUsers,
  })

  const reviewsQuery = useQuery({
    queryKey: ['admin', 'review-requests'],
    queryFn: () => reviewRequestsApi.getAll(),
  })

  const blockMutation = useMutation({
    mutationFn: ({ userId, reason }: { userId: string; reason: string }) =>
      adminApi.blockUser(userId, { reason }),
    onSuccess: () => void queryClient.invalidateQueries({ queryKey: ['admin'] }),
  })

  const unblockMutation = useMutation({
    mutationFn: adminApi.unblockUser,
    onSuccess: () => void queryClient.invalidateQueries({ queryKey: ['admin'] }),
  })

  const deleteReviewMutation = useMutation({
    mutationFn: adminApi.deleteReviewRequest,
    onSuccess: () => void queryClient.invalidateQueries({ queryKey: ['admin', 'review-requests'] }),
  })

  const isLoading = statsQuery.isLoading || usersQuery.isLoading

  if (isLoading) {
    return <Spinner label="Loading admin dashboard..." />
  }

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Admin dashboard</h1>
        <p className="mt-1 text-sm text-slate-500">System overview and user management</p>
      </div>

      {(statsQuery.error || usersQuery.error) && (
        <p className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">
          Failed to load admin data. Ensure you have admin privileges.
        </p>
      )}

      {statsQuery.data && <AdminStatsCards stats={statsQuery.data} />}

      <AdminTagManager />

      <section className="space-y-4">
        <h2 className="text-lg font-semibold text-slate-900">Users</h2>
        {usersQuery.data && (
          <AdminUserTable
            users={usersQuery.data}
            onBlock={async (userId, reason) => {
              await blockMutation.mutateAsync({ userId, reason })
            }}
            onUnblock={async (userId) => {
              await unblockMutation.mutateAsync(userId)
            }}
            isActionLoading={blockMutation.isPending || unblockMutation.isPending}
          />
        )}
      </section>

      <section className="space-y-4">
        <h2 className="text-lg font-semibold text-slate-900">Content moderation</h2>
        <p className="text-sm text-slate-500">Delete review requests that violate community guidelines.</p>
        {reviewsQuery.isLoading ? (
          <Spinner />
        ) : reviewsQuery.data?.length === 0 ? (
          <p className="text-sm text-slate-500">No review requests found.</p>
        ) : (
          <div className="rounded-xl border border-slate-200 bg-white overflow-hidden">
            <table className="w-full min-w-150 text-left text-sm">
              <thead className="border-b border-slate-200 bg-slate-50 text-slate-600">
                <tr>
                  <th className="px-4 py-3 font-medium">Title</th>
                  <th className="px-4 py-3 font-medium">Language</th>
                  <th className="px-4 py-3 font-medium">Status</th>
                  <th className="px-4 py-3 font-medium">Created</th>
                  <th className="px-4 py-3 font-medium">Action</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {reviewsQuery.data?.map((r) => (
                  <tr key={r.id} className="hover:bg-slate-50">
                    <td className="px-4 py-3 font-medium text-slate-900 max-w-xs truncate">{r.title}</td>
                    <td className="px-4 py-3 text-slate-600">{r.programmingLanguage}</td>
                    <td className="px-4 py-3 text-slate-600">{formatReviewStatus(r.status)}</td>
                    <td className="px-4 py-3 text-slate-500">{formatDate(r.createdAt)}</td>
                    <td className="px-4 py-3">
                      <button
                        type="button"
                        onClick={() => {
                          if (confirm(`Delete "${r.title}"?`)) {
                            deleteReviewMutation.mutate(r.id)
                          }
                        }}
                        className="text-sm font-medium text-red-600 hover:text-red-800"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  )
}
