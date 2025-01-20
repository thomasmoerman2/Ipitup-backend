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
    public Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Leaderboard>> GetTop10Async()
    {
        throw new NotImplementedException();
    }

    public Task<List<Leaderboard>> GetTop10ByLocationIdAsync(int locationId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetUserRankAsync(int userId, int? locationId = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateScoreAsync(int userId, int locationId, int newScore)
    {
        throw new NotImplementedException();
    }
}
