namespace Ipitup.Services;

public interface ILeaderboardService
{
    Task<bool> AddLeaderboardEntryAsync(Leaderboard leaderboard);
    Task<Leaderboard?> GetLeaderboardByIdAsync(int leaderboardId);
    Task<IEnumerable<Leaderboard>> GetLeaderboardByLocationIdAsync(int locationId);
    Task<IEnumerable<Leaderboard>> GetAllLeaderboardEntriesAsync();
    Task<bool> UpdateLeaderboardScoreAsync(int userId, int locationId, int activityScore); // Add this line
}


public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly IActivityRepository _activityRepository;

    public LeaderboardService(ILeaderboardRepository leaderboardRepository, IActivityRepository activityRepository)
    {
        _leaderboardRepository = leaderboardRepository;
        _activityRepository = activityRepository; 
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

    public async Task<IEnumerable<Leaderboard>> GetLeaderboardByLocationIdAsync(int locationId)
    {
        return await _leaderboardRepository.GetLeaderboardByLocationIdAsync(locationId);
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

}
