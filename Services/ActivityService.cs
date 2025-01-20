namespace Ipitup.Services;

public interface IActivityService
{
    Task<bool> AddActivityAsync(Activity activity);
    Task<IEnumerable<Activity>> GetAllActivitiesAsync();
    Task<Activity?> GetActivityByIdAsync(int id);
    Task<IEnumerable<Activity>> GetActivitiesByLocationIdAsync(int locationId);
    Task<List<Activity>> GetLatestActivityUserByIdAsync(int userId);
}

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;

    public ActivityService(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<bool> AddActivityAsync(Activity activity)
    {
        if (activity.UserId <= 0 || activity.ActivityScore < 0 || activity.ActivityDuration <= 0)
        {
            throw new ArgumentException("Invalid activity data");
        }
        return await _activityRepository.AddActivityAsync(activity);
    }

    public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        return await _activityRepository.GetAllActivitiesAsync();
    }

    public async Task<Activity?> GetActivityByIdAsync(int id)
    {
        return await _activityRepository.GetActivityByIdAsync(id);
    }

    public async Task<IEnumerable<Activity>> GetActivitiesByLocationIdAsync(int locationId)
{
    if (locationId <= 0)
    {
        throw new ArgumentException("Invalid location ID");
    }
    return await _activityRepository.GetActivitiesByLocationIdAsync(locationId);
}

    public async Task<List<Activity>> GetLatestActivityUserByIdAsync(int userId)
    {
        return await _activityRepository.GetLatestActivityUserByIdAsync(userId);
    }
}
