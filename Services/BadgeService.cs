namespace Ipitup.Services;

public interface IBadgeService
{
    Task<bool> AddBadgeAsync(Badge badge);
    Task<IEnumerable<Badge>> GetAllBadgesAsync();
    Task<Badge?> GetBadgeByIdAsync(int id);
    Task<IEnumerable<Badge>> GetBadgesByUserIdAsync(int userId);
    Task<bool> AddBadgeToUserAsync(int badgeId, int userId);
    Task<bool> DeleteBadgeAsync(int id);
    Task<bool> UpdateBadgeByIdAsync(int id, Badge badge);
}

public class BadgeService : IBadgeService
{
    private readonly IBadgeRepository _badgeRepository;

    public BadgeService(IBadgeRepository badgeRepository)
    {
        _badgeRepository = badgeRepository;
    }

    public async Task<bool> AddBadgeAsync(Badge badge)
    {
        if (string.IsNullOrWhiteSpace(badge.BadgeName))
        {
            throw new ArgumentException("Badge name is required");
        }

        return await _badgeRepository.AddBadgeAsync(badge);
    }

    public async Task<IEnumerable<Badge>> GetAllBadgesAsync()
    {
        return await _badgeRepository.GetAllBadgesAsync();
    }

    public async Task<Badge?> GetBadgeByIdAsync(int id)
    {
        return await _badgeRepository.GetBadgeByIdAsync(id);
    }

    public async Task<IEnumerable<Badge>> GetBadgesByUserIdAsync(int userId)
    {
        return await _badgeRepository.GetBadgesByUserIdAsync(userId);
    }

    public async Task<bool> AddBadgeToUserAsync(int badgeId, int userId)
    {
        return await _badgeRepository.AddBadgeToUserAsync(badgeId, userId);
    }

    public async Task<bool> DeleteBadgeAsync(int id)
    {
        return await _badgeRepository.DeleteBadgeAsync(id);
    }

    public async Task<bool> UpdateBadgeByIdAsync(int id, Badge badge)
    {
        return await _badgeRepository.UpdateBadgeByIdAsync(id, badge);
    }
}
