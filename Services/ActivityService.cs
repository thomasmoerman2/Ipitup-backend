namespace Ipitup.Services;
public interface IActivityService
{
    Task<bool> AddActivityAsync(Activity activity);
    Task<bool> DeleteActivityAsync(int id);
    Task<IEnumerable<Activity>> GetAllActivitiesAsync();
    Task<Activity?> GetActivityByIdAsync(int id);
    Task<IEnumerable<Activity>> GetActivitiesByLocationIdAsync(int locationId);
    Task<List<Activity>> GetLatestActivityUserByIdAsync(int userId);
    Task<bool> UpdateActivityByIdAsync(int id, Activity activity);
    Task<int> GetActivityCountByUserIdAsync(int userId);
}
public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IUserService _userService;
    private readonly ILeaderboardService _leaderboardService;
    public ActivityService(IActivityRepository activityRepository, IUserService userService, ILeaderboardService leaderboardService)
    {
        _activityRepository = activityRepository;
        _userService = userService;
        _leaderboardService = leaderboardService;
    }
    public async Task<bool> AddActivityAsync(Activity activity)
    {
        if (activity.UserId <= 0 || activity.ActivityScore < 0 || activity.ActivityDuration <= 0)
        {
            throw new ArgumentException("Invalid activity data");
        }
        var result = await _activityRepository.AddActivityAsync(activity);
        if (result)
        {
            // Update de totale gebruikersscore via UserService
            var updateUserSuccess = await _userService.UpdateUserTotalScoreAsync(activity.UserId, activity.ActivityScore);
            if (!updateUserSuccess)
            {
                throw new Exception($"Failed to update total score for user {activity.UserId}");
            }
            // Update de locatie-leaderboard via LeaderboardService
            if (activity.LocationId.HasValue)
            {
                var updateLeaderboardSuccess = await _leaderboardService.UpdateLeaderboardScoreAsync(activity.UserId, activity.LocationId.Value, activity.ActivityScore);
                if (!updateLeaderboardSuccess)
                {
                    throw new Exception($"Failed to update leaderboard score for location {activity.LocationId}");
                }
            }
        }
        return result;
    }
    public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        return await _activityRepository.GetAllActivitiesAsync();
    }
    public async Task<Activity?> GetActivityByIdAsync(int id)
    {
        return await _activityRepository.GetActivityByIdAsync(id);
    }
    public async Task<int> GetActivityCountByUserIdAsync(int userId)
    {
        return await _activityRepository.GetActivityCountByUserIdAsync(userId);
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
    public async Task<bool> DeleteActivityAsync(int id)
    {
        return await _activityRepository.DeleteActivityAsync(id);
    }
    public async Task<bool> UpdateActivityByIdAsync(int id, Activity activity)
    {
        return await _activityRepository.UpdateActivityByIdAsync(id, activity);
    }
}
