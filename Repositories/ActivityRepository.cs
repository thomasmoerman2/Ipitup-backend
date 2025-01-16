namespace Ipitup_backend.Repositories;

public interface IActivityRepository : IGenericRepository<Activity>
{
    Task<IEnumerable<Activity>> GetByUserIdAsync(int userId);
}

public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
{
    public ActivityRepository(Context context) : base(context) { }

    public async Task<IEnumerable<Activity>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(a => a.UserId == userId).ToListAsync();
    }
}