namespace Ipitup.Repositories;
public interface ILeaderboardRepository
{
    Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId);
    Task<List<Leaderboard>> GetTop10ByLocationIdAsync(int locationId);
    Task<List<Leaderboard>> GetTop10Async();
}
public class LeaderboardRepository : ILeaderboardRepository
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
}
