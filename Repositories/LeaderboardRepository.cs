namespace Ipitup_backend.Repositories;
public interface ILeaderboardRepository : IGenericRepository<Leaderboard>
{
    Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId);
    Task<List<Leaderboard>> GetTop10ByLocationIdAsync(int locationId);
    Task<List<Leaderboard>> GetTop10Async();
}
public class LeaderboardRepository : GenericRepository<Leaderboard>, ILeaderboardRepository
{
    public LeaderboardRepository(ApplicationContext context) : base(context) { }
    public async Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .Where(l => l.LocationId == locationId)
            .OrderByDescending(l => l.Score)
            .ToListAsync();
    }
    public async Task<List<Leaderboard>> GetTop10ByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .Where(l => l.LocationId == locationId)
            .OrderByDescending(l => l.Score)
            .Take(10)
            .ToListAsync();
    }
    public async Task<List<Leaderboard>> GetTop10Async()
    {
        return await _dbSet
            .OrderByDescending(l => l.Score)
            .Take(10)
            .ToListAsync();
    }
}
