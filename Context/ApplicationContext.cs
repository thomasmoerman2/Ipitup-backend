namespace Ipitup_backend.Context;
public class Context : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("SQLConnectionString"), ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("SQLConnectionString")));
    }
    // DbSet properties
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<BadgeUser> BadgeUsers { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Friends> Friends { get; set; }
    public DbSet<Leaderboard> Leaderboards { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserTotalScore> UserTotalScores { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure composite keys
        modelBuilder.Entity<BadgeUser>()
            .HasKey(bu => new { bu.BadgeId, bu.UserId });
        modelBuilder.Entity<Friends>()
            .HasKey(f => new { f.UserId, f.FriendId });
        // Configure relationships
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.Location)
            .WithMany()
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.Exercise)
            .WithMany()
            .HasForeignKey(a => a.ExerciseId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<BadgeUser>()
            .HasOne(bu => bu.Badge)
            .WithMany(b => b.BadgeUsers)
            .HasForeignKey(bu => bu.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BadgeUser>()
            .HasOne(bu => bu.User)
            .WithMany()
            .HasForeignKey(bu => bu.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Friends>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Friends>()
            .HasOne(f => f.FriendUser)
            .WithMany()
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Leaderboard>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Leaderboard>()
            .HasOne(l => l.Location)
            .WithMany()
            .HasForeignKey(l => l.LocationId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<UserTotalScore>()
            .HasOne(uts => uts.User)
            .WithOne()
            .HasForeignKey<UserTotalScore>(uts => uts.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Configure unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserEmail)
            .IsUnique();
        base.OnModelCreating(modelBuilder);
    }
}
