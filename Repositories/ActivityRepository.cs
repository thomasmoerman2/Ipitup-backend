namespace Ipitup_backend.Repositories;
public interface IActivityRepository : IGenericRepository<Activity>
{
    Task<IEnumerable<Activity>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Activity>> GetByLocationIdAsync(int locationId);
    Task<IEnumerable<Activity>> GetByUserIdAndLocationIdAsync(int userId, int locationId);
    Task<bool> AddNewActivityAsync(Activity activity);
    Task<bool> AddUserActivityAsync(int userId, int activityId);
}
public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
{
    public ActivityRepository(ApplicationContext context) : base(context) { }
    public async Task<IEnumerable<Activity>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(a => a.UserId == userId).ToListAsync();
    }
    public async Task<IEnumerable<Activity>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet.Where(a => a.LocationId == locationId).ToListAsync();
    }
    public async Task<IEnumerable<Activity>> GetByUserIdAndLocationIdAsync(int userId, int locationId)
    {
        return await _dbSet.Where(a => a.UserId == userId && a.LocationId == locationId).ToListAsync();
    }
    public async Task<bool> AddNewActivityAsync(Activity activity)
    {
        var result = await AddAsync(activity);
        return result != null;
    }
    public async Task<bool> AddUserActivityAsync(int userId, int activityId)
    {
        var result = await _context.ActivityUsers.AddAsync(new ActivityUser { UserId = userId, ActivityId = activityId });
        return result != null;
    }
}
