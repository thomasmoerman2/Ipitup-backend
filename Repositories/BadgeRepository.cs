namespace Ipitup.Repositories;
public interface IBadgeRepository
{
    Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId);
    Task<bool> AddNewBadgeAsync(Badge badge);
    Task<bool> AddUserBadgeAsync(int userId, int badgeId);
    Task<List<Badge>> GetAllBadgesAsync();
    Task<List<Badge>> GetLatest8BadgeByUserIdAsync(int userId);
}
public class BadgeRepository : IBadgeRepository
{
    public Task<bool> AddNewBadgeAsync(Badge badge)
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

    public Task<List<Badge>> GetLatest8BadgeByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
