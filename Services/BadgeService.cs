namespace Ipitup_backend.Services;
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
    private readonly IBadgeRepository _badgeRepository;
    public BadgeService(IBadgeRepository badgeRepository)
    {
        _badgeRepository = badgeRepository;
    }
    public async Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId)
    {
        return await _badgeRepository.GetUserBadgesAsync(userId);
    }
    public async Task<bool> AddBadgeAsync(Badge badge)
    {
        return await _badgeRepository.AddNewBadgeAsync(badge);
    }
    public async Task<bool> AddUserBadgeAsync(int userId, int badgeId)
    {
        // Check if user already has the badge
        if (await HasBadgeAsync(userId, badgeId))
            return false;
        return await _badgeRepository.AddUserBadgeAsync(userId, badgeId);
    }
    public async Task<List<Badge>> GetAllBadgesAsync()
    {
        return await _badgeRepository.GetAllBadgesAsync();
    }
    public async Task<List<Badge>> GetLatest8BadgesAsync(int userId)
    {
        return await _badgeRepository.GetLatest8BadgeByUserIdAsync(userId);
    }
    public async Task<bool> HasBadgeAsync(int userId, int badgeId)
    {
        var userBadges = await GetUserBadgesAsync(userId);
        return userBadges.Any(b => b.BadgeId == badgeId);
    }
}
