using DevReview.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevReview.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<ReviewRequest> ReviewRequests => Set<ReviewRequest>();
        public DbSet<CodeFile> CodeFiles => Set<CodeFile>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<OfficeHour> OfficeHours => Set<OfficeHour>();
        public DbSet<MentorReputation> MentorReputations => Set<MentorReputation>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<UserLanguage> UserLanguages => Set<UserLanguage>();
        public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
        public DbSet<UserFollow> UserFollows => Set<UserFollow>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Email).IsUnique();
                entity.HasIndex(x => x.UserName).IsUnique();
                entity.HasIndex(x => x.IsBlocked);
            });

            modelBuilder.Entity<ReviewRequest>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Owner).WithMany(x => x.ReviewRequests).HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Mentor).WithMany().HasForeignKey(x => x.MentorId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(x => x.Price).HasPrecision(10, 2);
            });

            modelBuilder.Entity<CodeFile>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.ReviewRequest).WithMany(x => x.CodeFiles).HasForeignKey(x => x.ReviewRequestId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.User).WithMany(x => x.Comments).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ReviewRequest).WithMany(x => x.Comments).HasForeignKey(x => x.ReviewRequestId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.CodeFile).WithMany().HasForeignKey(x => x.CodeFileId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.ParentComment).WithMany(x => x.Replies).HasForeignKey(x => x.ParentCommentId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Reviewer).WithMany(x => x.Reviews).HasForeignKey(x => x.ReviewerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ReviewRequest).WithMany(x => x.Reviews).HasForeignKey(x => x.ReviewRequestId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OfficeHour>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Mentor).WithMany(x => x.OfficeHours).HasForeignKey(x => x.MentorId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.BookedBy).WithMany().HasForeignKey(x => x.BookedById).OnDelete(DeleteBehavior.Restrict);
                entity.Property(x => x.Price).HasPrecision(10, 2);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<MentorReputation>(entity =>
            {
                entity.HasKey(x => new { x.MentorId, x.Language });
                entity.HasOne(x => x.Mentor).WithMany(x => x.MentorReputations).HasForeignKey(x => x.MentorId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(x => x.AverageScore).HasPrecision(4, 2);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.User).WithMany(x => x.Notifications).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserRefreshToken>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Token);
                entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserLanguage>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.User).WithMany(x => x.UserLanguages).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserFollow>(entity =>
            {
                entity.HasKey(x => new { x.FollowerId, x.MentorId });
                entity.HasOne(x => x.Follower).WithMany().HasForeignKey(x => x.FollowerId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Mentor).WithMany().HasForeignKey(x => x.MentorId).OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
