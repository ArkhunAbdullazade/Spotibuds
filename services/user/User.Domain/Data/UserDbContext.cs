using Microsoft.EntityFrameworkCore;
using User.Domain.Entities;

namespace User.Domain.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Don't configure User entity - Identity service manages Users table
        // User service only manages Follows table

        // Follow entity configuration
        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.FollowedId });
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key constraints to Users table (managed by Identity service)
            // Just define the constraints, don't create the table
            entity.HasIndex(e => e.FollowerId);
            entity.HasIndex(e => e.FollowedId);

            // Prevent self-following
            entity.ToTable(t => t.HasCheckConstraint("CK_Follow_NotSelf", "\"FollowerId\" != \"FollowedId\""));
        });
    }
} 