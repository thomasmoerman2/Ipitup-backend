namespace Ipitup.Services;
public interface IActivityService
{
    Task<IEnumerable<Activity>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Activity>> GetByLocationIdAsync(int locationId);
    Task<IEnumerable<Activity>> GetByUserIdAndLocationIdAsync(int userId, int locationId);
    Task<bool> AddActivityAsync(Activity activity);
    Task<bool> AddUserActivityAsync(int userId, int activityId);
    Task<bool> DeleteActivityAsync(int activityId);
    Task<Activity?> GetActivityByIdAsync(int activityId);
    Task<int> GetUserTotalScoreAsync(int userId);
}
public class ActivityService : IActivityService
{
    public Task<bool> AddActivityAsync(Activity activity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserActivityAsync(int userId, int activityId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteActivityAsync(int activityId)
    {
        throw new NotImplementedException();
    }

    public Task<Activity?> GetActivityByIdAsync(int activityId)
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

    public Task<int> GetUserTotalScoreAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
