export const APP_NAME = 'DevReview'

export const STORAGE_KEYS = {
  accessToken: 'devreview_access_token',
  refreshToken: 'devreview_refresh_token',
} as const

export const ROUTES = {
  home: '/',
  login: '/login',
  register: '/register',
  reviews: '/reviews',
  myReviews: '/reviews/mine',
  reviewDetail: (id: string) => `/reviews/${id}`,
  newReview: '/reviews/new',
  officeHours: '/office-hours',
  officeHoursSchedule: '/office-hours/schedule',
  notifications: '/notifications',
  admin: '/admin',
  mentors: '/mentors',
  mentorProfile: (id: string) => `/mentors/${id}`,
  knowledgeBase: '/knowledge-base',
  profile: '/profile',
} as const
