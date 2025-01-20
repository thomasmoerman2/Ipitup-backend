namespace Ipitup.Context;
using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("SQLConnectionString"), ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("SQLConnectionString")));
    }


    public DbSet<Activity> Activities { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<BadgeUser> BadgeUsers { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Friends> Friends { get; set; }
    public DbSet<Leaderboard> Leaderboards { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ActivityUser> ActivityUsers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BadgeUser>()
            .HasKey(bu => new { bu.BadgeId, bu.UserId });
        modelBuilder.Entity<Friends>()
            .HasKey(f => new { f.UserId, f.FriendId });
        modelBuilder.Entity<ActivityUser>()
            .HasKey(au => new { au.ActivityId, au.UserId });
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserEmail)
            .IsUnique();
        base.OnModelCreating(modelBuilder);
    }
}
