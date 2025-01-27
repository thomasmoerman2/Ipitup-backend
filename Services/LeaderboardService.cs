namespace Ipitup.Services;
public interface ILeaderboardService
{
    Task<bool> AddLeaderboardEntryAsync(Leaderboard leaderboard);
    Task<Leaderboard?> GetLeaderboardByIdAsync(int leaderboardId);
    Task<IEnumerable<Leaderboard>> GetAllLeaderboardEntriesAsync();
    Task<bool> UpdateLeaderboardScoreAsync(int userId, int locationId, int activityScore);
    Task<IEnumerable<dynamic>> GetLeaderboardWithFiltersAsync(List<int>? locationIds, int? minAge, int? maxAge, string? sortType, int userId);
    Task<int?> GetLeaderboardIdByUserIdAsync(int userId);
    Task<IEnumerable<User>> GetTopUsersByTotalScoreAsync(int top);
    Task<int> GetTotalLocationScoreByUserAsync(int userId, List<int> locationIds);

}
public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IUserRepository _userRepository;
    public LeaderboardService(ILeaderboardRepository leaderboardRepository, IActivityRepository activityRepository, IUserRepository userRepository)
    {
        _leaderboardRepository = leaderboardRepository;
        _activityRepository = activityRepository;
        _userRepository = userRepository;
    }
    public async Task<bool> AddLeaderboardEntryAsync(Leaderboard leaderboard)
    {
        if (leaderboard.UserId <= 0 || leaderboard.Score < 0)
        {
            throw new ArgumentException("Invalid leaderboard data");
        }
        return await _leaderboardRepository.AddLeaderboardEntryAsync(leaderboard);
    }
    public async Task<Leaderboard?> GetLeaderboardByIdAsync(int leaderboardId)
    {
        return await _leaderboardRepository.GetLeaderboardByIdAsync(leaderboardId);
    }

    public async Task<int> GetTotalLocationScoreByUserAsync(int userId, List<int> locationIds)
    {
        return await _leaderboardRepository.GetTotalLocationScoreByUserAsync(userId, locationIds);
    }


    public async Task<IEnumerable<Leaderboard>> GetAllLeaderboardEntriesAsync()
    {
        return await _leaderboardRepository.GetAllLeaderboardEntriesAsync();
    }
    public async Task<bool> AddActivityAsync(Activity activity)
    {
        var result = await _activityRepository.AddActivityAsync(activity);
        if (result)
        {
            await UpdateLeaderboardScoreAsync(activity.UserId, activity.LocationId ?? 0, activity.ActivityScore);
        }
        return result;
    }
    public async Task<bool> UpdateLeaderboardScoreAsync(int userId, int locationId, int activityScore)
    {
        return await _leaderboardRepository.UpdateLeaderboardScoreAsync(userId, locationId, activityScore);
    }
    public async Task<IEnumerable<dynamic>> GetLeaderboardWithFiltersAsync(List<int>? locationIds, int? minAge, int? maxAge, string? sortType, int userId)
    {
        return await _leaderboardRepository.GetLeaderboardWithFiltersAsync(locationIds, minAge, maxAge, sortType, userId);
    }
    public async Task<IEnumerable<User>> GetTopUsersByTotalScoreAsync(int top)
    {
        if (top <= 0)
        {
            throw new ArgumentException("Top must be greater than 0");
        }
        var users = await _userRepository.GetAllUsersAsync();
        var leaderboards = await _leaderboardRepository.GetAllLeaderboardEntriesAsync();
        var userScores = users.Select(u => new
        {
            UserId = u.UserId,
            TotalScore = leaderboards.Where(l => l.UserId == u.UserId).Sum(l => l.Score)
        });
        var topUsers = userScores.OrderByDescending(u => u.TotalScore).Take(top);
        var topUserIds = topUsers.Select(u => u.UserId).ToList();
        return users.Where(u => topUserIds.Contains(u.UserId));
    }
    public async Task<int?> GetLeaderboardIdByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Invalid userId provided.");
        }
        return await _leaderboardRepository.GetLeaderboardIdByUserIdAsync(userId);
    }
}
