import { Link } from 'react-router-dom'
import { Card } from '@/components/common/Card'
import { Button } from '@/components/common/Button'
import { UserAvatar } from './UserAvatar'
import { useAuth } from '@/hooks/useAuth'
import type { MentorRanking } from '@/types'
import { isKnownLanguage } from '@/utils/languages'
import { ROUTES } from '@/utils/constants'

interface MentorCardProps {
  mentor: MentorRanking
  isFollowing?: boolean
  onFollow?: (mentorId: string) => void
  onUnfollow?: (mentorId: string) => void
  isFollowLoading?: boolean
}

export function MentorCard({
  mentor,
  isFollowing = false,
  onFollow,
  onUnfollow,
  isFollowLoading = false,
}: MentorCardProps) {
  const { isAuthenticated, user } = useAuth()

  return (
    <Link to={ROUTES.mentorProfile(mentor.mentorId)} className="block">
      <Card className="h-full transition-shadow hover:shadow-md cursor-pointer">
        <div className="flex items-start gap-3">
          <UserAvatar name={mentor.displayName} size="md" />
          <div className="min-w-0 flex-1">
            <div className="flex items-center justify-between gap-2">
              <span className="font-semibold text-slate-900">{mentor.displayName}</span>
              {isAuthenticated && mentor.mentorId !== user?.id && onFollow && onUnfollow && (
                <span onClick={(e) => e.preventDefault()}>
                  <Button
                    size="sm"
                    variant={isFollowing ? 'secondary' : 'primary'}
                    isLoading={isFollowLoading}
                    onClick={() => isFollowing ? onUnfollow(mentor.mentorId) : onFollow(mentor.mentorId)}
                  >
                    {isFollowing ? 'Unfollow' : 'Follow'}
                  </Button>
                </span>
              )}
            </div>
            <p className="text-sm text-slate-500">@{mentor.userName}</p>
            {mentor.bio && (
              <p className="mt-1 text-sm text-slate-600 line-clamp-2">{mentor.bio}</p>
            )}
            <dl className="mt-3 grid grid-cols-2 gap-2 text-sm">
              <div>
                <dt className="text-slate-500">Rating</dt>
                <dd className="font-medium text-slate-800">{mentor.overallRating.toFixed(1)} / 5</dd>
              </div>
              <div>
                <dt className="text-slate-500">Reviews</dt>
                <dd className="font-medium text-slate-800">{mentor.totalReviews}</dd>
              </div>
              <div>
                <dt className="text-slate-500">Experience</dt>
                <dd className="font-medium text-slate-800">{mentor.yearsOfExperience} yrs</dd>
              </div>
              {mentor.hourlyRate != null && (
                <div>
                  <dt className="text-slate-500">Hourly rate</dt>
                  <dd className="font-medium text-slate-800">€{mentor.hourlyRate}/h</dd>
                </div>
              )}
              {mentor.availableHoursPerWeek != null && mentor.availableHoursPerWeek > 0 && (
                <div>
                  <dt className="text-slate-500">Availability</dt>
                  <dd className="font-medium text-slate-800">{mentor.availableHoursPerWeek}h/week</dd>
                </div>
              )}
            </dl>
            {(() => {
              const langExpertise = mentor.languagesExpertise.filter(l => isKnownLanguage(l.language))
              if (langExpertise.length > 0) return (
                <div className="mt-3 flex flex-wrap gap-1.5">
                  {langExpertise.map((lang) => (
                    <span
                      key={`${lang.language}-${lang.score}`}
                      className="rounded-md bg-violet-100 px-2 py-0.5 text-xs text-violet-700"
                    >
                      {lang.language} ({lang.score.toFixed(1)}★)
                    </span>
                  ))}
                </div>
              )
              return null
            })()}
            {mentor.profileLanguages?.length > 0 ? (
              <div className="mt-3 flex flex-wrap gap-1.5">
                {mentor.profileLanguages.map((lang) => (
                  <span
                    key={lang}
                    className="rounded-md bg-slate-100 px-2 py-0.5 text-xs text-slate-700"
                  >
                    {lang}
                  </span>
                ))}
              </div>
            ) : null}
          </div>
        </div>
      </Card>
    </Link>
  )
}
