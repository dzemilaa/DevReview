import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { tagsApi } from '@/api'
import { Button } from '@/components/common/Button'
import { Card } from '@/components/common/Card'
import { Input } from '@/components/common/Input'
import { Spinner } from '@/components/common/Spinner'
import type { Tag } from '@/types'

export function AdminTagManager() {
  const queryClient = useQueryClient()
  const [newName, setNewName] = useState('')
  const [editingId, setEditingId] = useState<string | null>(null)
  const [editName, setEditName] = useState('')

  const { data: tags = [], isLoading } = useQuery({
    queryKey: ['tags'],
    queryFn: tagsApi.getAll,
  })

  const createMutation = useMutation({
    mutationFn: tagsApi.create,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['tags'] })
      setNewName('')
    },
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, name }: { id: string; name: string }) => tagsApi.update(id, { name }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['tags'] })
      setEditingId(null)
    },
  })

  const deleteMutation = useMutation({
    mutationFn: tagsApi.delete,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['tags'] })
    },
  })

  if (isLoading) return <Spinner label="Loading tags..." />

  return (
    <Card>
      <h2 className="text-lg font-semibold text-slate-900">Tags</h2>
      <form
        className="mt-4 flex flex-wrap gap-2"
        onSubmit={(e) => {
          e.preventDefault()
          if (!newName.trim()) return
          createMutation.mutate({ name: newName.trim() })
        }}
      >
        <Input
          label="New tag"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          className="min-w-[200px] flex-1"
        />
        <div className="flex items-end">
          <Button type="submit" isLoading={createMutation.isPending}>
            Add tag
          </Button>
        </div>
      </form>

      <ul className="mt-6 divide-y divide-slate-100">
        {tags.map((tag: Tag) => (
          <li key={tag.id} className="flex flex-wrap items-center justify-between gap-2 py-3">
            {editingId === tag.id ? (
              <div className="flex flex-1 flex-wrap gap-2">
                <Input value={editName} onChange={(e) => setEditName(e.target.value)} />
                <Button
                  size="sm"
                  onClick={() => updateMutation.mutate({ id: tag.id, name: editName.trim() })}
                  isLoading={updateMutation.isPending}
                >
                  Save
                </Button>
                <Button size="sm" variant="ghost" onClick={() => setEditingId(null)}>
                  Cancel
                </Button>
              </div>
            ) : (
              <>
                <span className="font-medium text-slate-800">#{tag.name}</span>
                <div className="flex gap-2">
                  <Button
                    size="sm"
                    variant="secondary"
                    onClick={() => {
                      setEditingId(tag.id)
                      setEditName(tag.name)
                    }}
                  >
                    Edit
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    isLoading={deleteMutation.isPending}
                    onClick={() => {
                      if (confirm(`Delete tag "${tag.name}"?`)) {
                        deleteMutation.mutate(tag.id)
                      }
                    }}
                  >
                    Delete
                  </Button>
                </div>
              </>
            )}
          </li>
        ))}
      </ul>
    </Card>
  )
}
