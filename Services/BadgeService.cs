namespace Ipitup.Services;
public interface IBadgeService
{
    Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId);
    Task<bool> AddBadgeAsync(Badge badge);
    Task<bool> AddUserBadgeAsync(int userId, int badgeId);
    Task<List<Badge>> GetAllBadgesAsync();
    Task<List<Badge>> GetLatest8BadgesAsync(int userId);
    Task<bool> HasBadgeAsync(int userId, int badgeId);
}
public class BadgeService : IBadgeService
{
    public Task<bool> AddBadgeAsync(Badge badge)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserBadgeAsync(int userId, int badgeId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Badge>> GetAllBadgesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Badge>> GetLatest8BadgesAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasBadgeAsync(int userId, int badgeId)
    {
        throw new NotImplementedException();
    }
}
