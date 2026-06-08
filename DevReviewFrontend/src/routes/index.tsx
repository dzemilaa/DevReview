import { createBrowserRouter, Navigate } from 'react-router-dom'
import { AppLayout } from '@/layouts/AppLayout'
import { AuthLayout } from '@/layouts/AuthLayout'
import { HomePage } from '@/pages/HomePage'
import { LoginPage } from '@/pages/LoginPage'
import { RegisterPage } from '@/pages/RegisterPage'
import { ReviewRequestsPage } from '@/pages/ReviewRequestsPage'
import { MyReviewRequestsPage } from '@/pages/MyReviewRequestsPage'
import { ReviewRequestDetailPage } from '@/pages/ReviewRequestDetailPage'
import { NewReviewRequestPage } from '@/pages/NewReviewRequestPage'
import { NotificationsPage } from '@/pages/NotificationsPage'
import { OfficeHoursPage } from '@/pages/OfficeHoursPage'
import { MentorOfficeHoursPage } from '@/pages/MentorOfficeHoursPage'
import { AdminDashboardPage } from '@/pages/AdminDashboardPage'
import { MentorsPage } from '@/pages/MentorsPage'
import { MentorProfilePage } from '@/pages/MentorProfilePage'
import { ProfilePage } from '@/pages/ProfilePage'
import { KnowledgeBasePage } from '@/pages/KnowledgeBasePage'
import { NotFoundPage } from '@/pages/NotFoundPage'
import { ProtectedRoute } from './ProtectedRoute'
import { AdminRoute } from './AdminRoute'
import { MentorRoute } from './MentorRoute'
import { ROUTES } from '@/utils/constants'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: 'office-hours', element: <OfficeHoursPage /> },
      { path: 'knowledge-base', element: <KnowledgeBasePage /> },
      { path: 'mentors', element: <MentorsPage /> },
      { path: 'mentors/:id', element: <MentorProfilePage /> },
      { path: 'reviews/:id', element: <ReviewRequestDetailPage /> },
      {
        element: <ProtectedRoute />,
        children: [
          { path: 'reviews', element: <ReviewRequestsPage /> },
          { path: 'reviews/mine', element: <MyReviewRequestsPage /> },
          { path: 'reviews/new', element: <NewReviewRequestPage /> },
          { path: 'notifications', element: <NotificationsPage /> },
          { path: 'profile', element: <ProfilePage /> },
        ],
      },
      {
        element: <MentorRoute />,
        children: [{ path: 'office-hours/schedule', element: <MentorOfficeHoursPage /> }],
      },
      {
        element: <AdminRoute />,
        children: [{ path: 'admin', element: <AdminDashboardPage /> }],
      },
      { path: '404', element: <NotFoundPage /> },
      { path: '*', element: <Navigate to="/404" replace /> },
    ],
  },
  {
    element: <AuthLayout />,
    children: [
      { path: ROUTES.login.slice(1), element: <LoginPage /> },
      { path: ROUTES.register.slice(1), element: <RegisterPage /> },
    ],
  },
])
