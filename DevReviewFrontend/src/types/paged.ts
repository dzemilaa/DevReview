export interface PagedResult<T> {
  totalItems: number
  totalPages: number
  currentPage: number
  pageSize: number
  items: T[]
}
