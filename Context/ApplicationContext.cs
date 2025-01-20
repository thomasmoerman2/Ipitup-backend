namespace Ipitup_backend.Context;
public class ApplicationContext : DbContext
{
    private readonly IConfiguration _configuration;
    public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString));
        }
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
