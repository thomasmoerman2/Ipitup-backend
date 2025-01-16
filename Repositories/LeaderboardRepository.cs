namespace Ipitup_backend.Repositories;

public interface ILeaderboardRepository : IGenericRepository<Leaderboard>
{
    Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId);
}

public class LeaderboardRepository : GenericRepository<Leaderboard>, ILeaderboardRepository
{
    public LeaderboardRepository(Context context) : base(context) { }

    public async Task<IEnumerable<Leaderboard>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .Where(l => l.LocationId == locationId)
            .OrderByDescending(l => l.Score)
            .ToListAsync();
    }
}