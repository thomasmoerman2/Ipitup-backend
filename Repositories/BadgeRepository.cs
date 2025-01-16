namespace Ipitup_backend.Repositories;

public interface IBadgeRepository : IGenericRepository<Badge>
{
    Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId);
}

public class BadgeRepository : GenericRepository<Badge>, IBadgeRepository
{
    public BadgeRepository(Context context) : base(context) { }

    public async Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId)
    {
        return await _context.BadgeUsers
            .Where(bu => bu.UserId == userId)
            .Select(bu => bu.Badge!)
            .ToListAsync();
    }
}