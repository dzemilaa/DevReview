import { useState } from 'react'
import { Badge } from '@/components/common/Badge'
import { Button } from '@/components/common/Button'
import { Input } from '@/components/common/Input'
import { Card } from '@/components/common/Card'
import type { AdminUser } from '@/types'
import { formatDate } from '@/utils/format'

interface AdminUserTableProps {
  users: AdminUser[]
  onBlock: (userId: string, reason: string) => Promise<void>
  onUnblock: (userId: string) => Promise<void>
  isActionLoading?: boolean
}

export function AdminUserTable({
  users,
  onBlock,
  onUnblock,
  isActionLoading = false,
}: AdminUserTableProps) {
  const [blockingId, setBlockingId] = useState<string | null>(null)
  const [blockReason, setBlockReason] = useState('')

  return (
    <Card padding="none" className="overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full min-w-[640px] text-left text-sm">
          <thead className="border-b border-slate-200 bg-slate-50 text-slate-600">
            <tr>
              <th className="px-4 py-3 font-medium">User</th>
              <th className="px-4 py-3 font-medium">Role</th>
              <th className="px-4 py-3 font-medium">Status</th>
              <th className="px-4 py-3 font-medium">Reviews</th>
              <th className="px-4 py-3 font-medium">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {users.map((user) => (
              <tr key={user.id} className="hover:bg-slate-50/50">
                <td className="px-4 py-3">
                  <p className="font-medium text-slate-900">{user.displayName}</p>
                  <p className="text-xs text-slate-500">
                    @{user.userName} · {user.email}
                  </p>
                </td>
                <td className="px-4 py-3">
                  <Badge variant="neutral">{user.role}</Badge>
                </td>
                <td className="px-4 py-3">
                  {user.isBlocked ? (
                    <div>
                      <Badge variant="warning">Blocked</Badge>
                      {user.blockReason && (
                        <p className="mt-1 text-xs text-slate-500">{user.blockReason}</p>
                      )}
                      {user.blockedAt && (
                        <p className="text-xs text-slate-400">{formatDate(user.blockedAt)}</p>
                      )}
                    </div>
                  ) : user.isActive ? (
                    <Badge variant="success">Active</Badge>
                  ) : (
                    <Badge variant="neutral">Inactive</Badge>
                  )}
                </td>
                <td className="px-4 py-3 text-slate-600">{user.totalReviews}</td>
                <td className="px-4 py-3">
                  {user.isBlocked ? (
                    <Button
                      size="sm"
                      variant="secondary"
                      isLoading={isActionLoading}
                      onClick={() => void onUnblock(user.id)}
                    >
                      Unblock
                    </Button>
                  ) : blockingId === user.id ? (
                    <div className="flex min-w-[200px] flex-col gap-2">
                      <Input
                        value={blockReason}
                        onChange={(e) => setBlockReason(e.target.value)}
                        placeholder="Block reason"
                      />
                      <div className="flex gap-2">
                        <Button
                          size="sm"
                          variant="danger"
                          isLoading={isActionLoading}
                          disabled={!blockReason.trim()}
                          onClick={() =>
                            void onBlock(user.id, blockReason).then(() => {
                              setBlockingId(null)
                              setBlockReason('')
                            })
                          }
                        >
                          Confirm
                        </Button>
                        <Button size="sm" variant="ghost" onClick={() => setBlockingId(null)}>
                          Cancel
                        </Button>
                      </div>
                    </div>
                  ) : (
                    <Button size="sm" variant="danger" onClick={() => setBlockingId(user.id)}>
                      Block
                    </Button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </Card>
  )
}
