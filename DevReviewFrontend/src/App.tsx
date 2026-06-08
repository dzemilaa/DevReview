import { useEffect } from 'react'
import { RouterProvider } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useAuthStore } from '@/store/authStore'
import { router } from '@/routes'

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
})

function AppInit({ children }: { children: React.ReactNode }) {
  const initialize = useAuthStore((s) => s.initialize)
  useEffect(() => { void initialize() }, [initialize])
  return children
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppInit>
        <RouterProvider router={router} />
      </AppInit>
    </QueryClientProvider>
  )
}

export default App
