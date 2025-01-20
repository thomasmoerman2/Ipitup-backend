namespace Ipitup.Services;
public interface ILeaderboardService
{
    Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId);
    Task<List<Leaderboard>> GetTop10ByLocationIdAsync(int locationId);
    Task<List<Leaderboard>> GetTop10Async();
    Task<bool> UpdateScoreAsync(int userId, int locationId, int newScore);
    Task<int> GetUserRankAsync(int userId, int? locationId = null);
}
public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    public LeaderboardService(ILeaderboardRepository leaderboardRepository)
    {
        _leaderboardRepository = leaderboardRepository;
    }
    public async Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId)
    {
        return await _leaderboardRepository.GetByLocationIdAsync(locationId);
    }
    public async Task<List<Leaderboard>> GetTop10ByLocationIdAsync(int locationId)
    {
        return await _leaderboardRepository.GetTop10ByLocationIdAsync(locationId);
    }
    public async Task<List<Leaderboard>> GetTop10Async()
    {
        return await _leaderboardRepository.GetTop10Async();
    }
    public async Task<bool> UpdateScoreAsync(int userId, int locationId, int newScore)
    {
        var leaderboard = (await _leaderboardRepository.GetByLocationIdAsync(locationId))
            .FirstOrDefault(l => l.UserId == userId);
        if (leaderboard == null)
        {
            leaderboard = new Leaderboard
            {
                UserId = userId,
                LocationId = locationId,
                Score = newScore
            };
            await _leaderboardRepository.AddAsync(leaderboard);
        }
        else
        {
            leaderboard.Score = newScore;
            await _leaderboardRepository.UpdateAsync(leaderboard);
        }
        return true;
    }
    public async Task<int> GetUserRankAsync(int userId, int? locationId = null)
    {
        var leaderboards = locationId.HasValue
            ? await GetByLocationIdAsync(locationId.Value)
            : await _leaderboardRepository.GetAllAsync();
        var orderedLeaderboards = leaderboards
            .OrderByDescending(l => l.Score)
            .ToList();
        var userRank = orderedLeaderboards
            .FindIndex(l => l.UserId == userId) + 1;
        return userRank == 0 ? -1 : userRank;
    }
}
