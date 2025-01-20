namespace Ipitup.Repositories;
public interface IActivityRepository
{
    Task<IEnumerable<Activity>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Activity>> GetByLocationIdAsync(int locationId);
    Task<IEnumerable<Activity>> GetByUserIdAndLocationIdAsync(int userId, int locationId);
    Task<bool> AddNewActivityAsync(Activity activity);
    Task<bool> AddUserActivityAsync(int userId, int activityId);
}
public class ActivityRepository : IActivityRepository
{
    public Task<bool> AddNewActivityAsync(Activity activity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserActivityAsync(int userId, int activityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Activity>> GetByLocationIdAsync(int locationId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Activity>> GetByUserIdAndLocationIdAsync(int userId, int locationId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Activity>> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
