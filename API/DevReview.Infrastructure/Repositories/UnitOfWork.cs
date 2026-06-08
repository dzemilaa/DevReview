using System.Threading;
using System.Threading.Tasks;
using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;
using DevReview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevReview.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(_context);
            ReviewRequests = new GenericRepository<ReviewRequest>(_context);
            CodeFiles = new GenericRepository<CodeFile>(_context);
            Comments = new GenericRepository<Comment>(_context);
            Reviews = new GenericRepository<Review>(_context);
            OfficeHours = new GenericRepository<OfficeHour>(_context);
            Tags = new GenericRepository<Tag>(_context);
            MentorReputations = new GenericRepository<MentorReputation>(_context);
            Notifications = new GenericRepository<Notification>(_context);
            UserLanguages = new GenericRepository<UserLanguage>(_context);
            UserRefreshTokens = new GenericRepository<UserRefreshToken>(_context);
            UserFollows = new GenericRepository<UserFollow>(_context);
        }

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<ReviewRequest> ReviewRequests { get; }
        public IGenericRepository<CodeFile> CodeFiles { get; }
        public IGenericRepository<Comment> Comments { get; }
        public IGenericRepository<Review> Reviews { get; }
        public IGenericRepository<OfficeHour> OfficeHours { get; }
        public IGenericRepository<Tag> Tags { get; }
        public IGenericRepository<MentorReputation> MentorReputations { get; }
        public IGenericRepository<Notification> Notifications { get; }
        public IGenericRepository<UserLanguage> UserLanguages { get; }
        public IGenericRepository<UserRefreshToken> UserRefreshTokens { get; }
        public IGenericRepository<UserFollow> UserFollows { get; }

        public async Task<ReviewRequest?> GetReviewRequestWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ReviewRequests
                .Include(x => x.CodeFiles)
                .Include(x => x.Comments).ThenInclude(c => c.User)
                .Include(x => x.Reviews)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
