namespace Ipitup_backend.Services;
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
    private readonly IActivityRepository _activityRepository;
    public ActivityService(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }
    public async Task<IEnumerable<Activity>> GetByUserIdAsync(int userId)
    {
        return await _activityRepository.GetByUserIdAsync(userId);
    }
    public async Task<IEnumerable<Activity>> GetByLocationIdAsync(int locationId)
    {
        return await _activityRepository.GetByLocationIdAsync(locationId);
    }
    public async Task<IEnumerable<Activity>> GetByUserIdAndLocationIdAsync(int userId, int locationId)
    {
        return await _activityRepository.GetByUserIdAndLocationIdAsync(userId, locationId);
    }
    public async Task<bool> AddActivityAsync(Activity activity)
    {
        return await _activityRepository.AddNewActivityAsync(activity);
    }
    public async Task<bool> AddUserActivityAsync(int userId, int activityId)
    {
        return await _activityRepository.AddUserActivityAsync(userId, activityId);
    }
    public async Task<bool> DeleteActivityAsync(int activityId)
    {
        var activity = await GetActivityByIdAsync(activityId);
        if (activity == null)
            return false;
        await _activityRepository.DeleteAsync(activityId);
        return true;
    }
    public async Task<Activity?> GetActivityByIdAsync(int activityId)
    {
        return await _activityRepository.GetByIdAsync(activityId);
    }
    public async Task<int> GetUserTotalScoreAsync(int userId)
    {
        var activities = await GetByUserIdAsync(userId);
        return activities.Sum(a => a.ActivityScore);
    }
}
