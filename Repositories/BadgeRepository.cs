namespace Ipitup.Repositories;
public interface IBadgeRepository : IGenericRepository<Badge>
{
    Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId);
    Task<bool> AddNewBadgeAsync(Badge badge);
    Task<bool> AddUserBadgeAsync(int userId, int badgeId);
    Task<List<Badge>> GetAllBadgesAsync();
    Task<List<Badge>> GetLatest8BadgeByUserIdAsync(int userId);
}
public class BadgeRepository : GenericRepository<Badge>, IBadgeRepository
{
    public BadgeRepository(ApplicationContext context) : base(context) { }
    public async Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId)
    {
        return await _context.BadgeUsers
            .Where(bu => bu.UserId == userId)
            .Select(bu => bu.Badge!)
            .ToListAsync();
    }
    public async Task<bool> AddNewBadgeAsync(Badge badge)
    {
        var result = await AddAsync(badge);
        return result != null;
    }
    public async Task<bool> AddUserBadgeAsync(int userId, int badgeId)
    {
        var result = await _context.BadgeUsers.AddAsync(new BadgeUser { UserId = userId, BadgeId = badgeId });
        return result != null;
    }
    public async Task<List<Badge>> GetAllBadgesAsync()
    {
        return await _dbSet.ToListAsync();
    }
    public async Task<List<Badge>> GetLatest8BadgeByUserIdAsync(int userId)
    {
        return await _context.BadgeUsers
            .Where(bu => bu.UserId == userId)
            .OrderByDescending(bu => bu.BadgeId)
            .Select(bu => bu.Badge!)
            .Take(8)
            .ToListAsync();
    }
}
