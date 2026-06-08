import { Button } from '@/components/common/Button'
import { Input } from '@/components/common/Input'
import type { SearchReviewRequestsParams } from '@/types'
import { DifficultyLevel, ReviewStatus } from '@/types'

interface ReviewSearchFiltersProps {
  filters: SearchReviewRequestsParams
  onChange: (filters: SearchReviewRequestsParams) => void
  onSearch: () => void
}

function getPricePreset(minPrice?: number, maxPrice?: number): string {
  if (maxPrice === 0) return 'free'
  if (minPrice != null && minPrice > 0) return 'paid'
  return ''
}

export function ReviewSearchFilters({ filters, onChange, onSearch }: ReviewSearchFiltersProps) {
  const set = (partial: Partial<SearchReviewRequestsParams>) => onChange({ ...filters, ...partial })

  const handlePriceChange = (value: string) => {
    if (value === 'free') set({ minPrice: undefined, maxPrice: 0 })
    else if (value === 'paid') set({ minPrice: 0.01, maxPrice: undefined })
    else set({ minPrice: undefined, maxPrice: undefined })
  }

  return (
    <form
      onSubmit={(e) => { e.preventDefault(); onSearch() }}
      className="rounded-xl border border-slate-200 bg-white p-4"
    >
      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
        <Input
          label="Language"
          value={filters.searchLanguage ?? ''}
          onChange={(e) => set({ searchLanguage: e.target.value || undefined })}
          placeholder="e.g. TypeScript, Python..."
        />
        <Input
          label="Framework"
          value={filters.framework ?? ''}
          onChange={(e) => set({ framework: e.target.value || undefined })}
          placeholder="e.g. React, ASP.NET..."
        />
        <Input
          label="Tag"
          value={filters.searchTag ?? ''}
          onChange={(e) => set({ searchTag: e.target.value || undefined })}
          placeholder="e.g. api, refactor..."
        />
        <div className="space-y-1.5">
          <label htmlFor="search-difficulty" className="block text-sm font-medium text-slate-700">
            Difficulty
          </label>
          <select
            id="search-difficulty"
            value={filters.searchDifficulty ?? ''}
            onChange={(e) =>
              set({
                searchDifficulty:
                  e.target.value === '' ? undefined : (Number(e.target.value) as DifficultyLevel),
              })
            }
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm"
          >
            <option value="">All difficulties</option>
            <option value={DifficultyLevel.Junior}>Junior</option>
            <option value={DifficultyLevel.Mid}>Mid</option>
            <option value={DifficultyLevel.Senior}>Senior</option>
          </select>
        </div>
        <div className="space-y-1.5">
          <label htmlFor="search-price" className="block text-sm font-medium text-slate-700">
            Price
          </label>
          <select
            id="search-price"
            value={getPricePreset(filters.minPrice, filters.maxPrice)}
            onChange={(e) => handlePriceChange(e.target.value)}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm"
          >
            <option value="">All prices</option>
            <option value="free">Free</option>
            <option value="paid">Paid</option>
          </select>
        </div>
        <div className="space-y-1.5">
          <label htmlFor="search-status" className="block text-sm font-medium text-slate-700">
            Status
          </label>
          <select
            id="search-status"
            value={filters.status ?? ''}
            onChange={(e) =>
              set({
                status: e.target.value === '' ? undefined : (Number(e.target.value) as ReviewStatus),
              })
            }
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm"
          >
            <option value={ReviewStatus.Open}>Open</option>
            <option value={ReviewStatus.Claimed}>Claimed</option>
            <option value={ReviewStatus.InReview}>In review</option>
            <option value={ReviewStatus.Completed}>Completed</option>
            <option value={ReviewStatus.Abandoned}>Abandoned</option>
            <option value="">All statuses</option>
          </select>
        </div>
      </div>

      <div className="mt-3 flex items-center gap-3">
        <Button type="submit">Search</Button>
        <button
          type="button"
          onClick={() => onChange({ page: filters.page, pageSize: filters.pageSize })}
          className="text-sm text-slate-500 hover:text-slate-700"
        >
          Reset
        </button>
      </div>
    </form>
  )
}
