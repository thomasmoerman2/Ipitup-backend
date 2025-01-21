namespace Ipitup.Functions;

public class ActivityTrigger
{
    private readonly ILogger<ActivityTrigger> _logger;
    private readonly IActivityService _activityService;

    public ActivityTrigger(ILogger<ActivityTrigger> logger, IActivityService activityService)
    {
        _logger = logger;
        _activityService = activityService;
    }

    [Function("PostActivity")]
    public async Task<IActionResult> PostActivity(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activity/add")] HttpRequest req)
    {
        _logger.LogInformation("PostActivity function triggered");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var activityRequest = JsonConvert.DeserializeObject<Activity>(requestBody);

        if (activityRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _activityService.AddActivityAsync(activityRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to add activity" });
        }

        return new OkObjectResult(new { message = "Activity added successfully" });
    }

    [Function("UpdateActivityById")]
    public async Task<IActionResult> UpdateActivityById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "activity/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int activityId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var activityRequest = JsonConvert.DeserializeObject<Activity>(requestBody);

        if (activityRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _activityService.UpdateActivityByIdAsync(activityId, activityRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to update activity" });
        }

        return new OkObjectResult(new { message = "Activity updated successfully" });
    }
    [Function("RemoveActivityById")]
    public async Task<IActionResult> RemoveActivityById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "activity/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int activityId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var result = await _activityService.DeleteActivityAsync(activityId);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to remove activity" });
        }

        return new OkObjectResult(new { message = "Activity removed successfully" });
    }

    [Function("GetAllActivities")]
    public async Task<IActionResult> GetAllActivities(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activity")] HttpRequest req)
    {
        var activities = await _activityService.GetAllActivitiesAsync();
        return new OkObjectResult(activities);
    }

    [Function("GetActivityById")]
    public async Task<IActionResult> GetActivityById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activity/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int activityId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var activity = await _activityService.GetActivityByIdAsync(activityId);
        if (activity == null)
        {
            return new NotFoundObjectResult(new { message = "Activity not found" });
        }

        return new OkObjectResult(activity);
    }

    [Function("GetActivitiesByLocation")]
    public async Task<IActionResult> GetActivitiesByLocation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activity/location/{locationId}")] HttpRequest req, string locationId)
    {
        if (!int.TryParse(locationId, out int locId))
        {
            return new BadRequestObjectResult(new { message = "Invalid location ID format. It must be a number." });
        }

        var activities = await _activityService.GetActivitiesByLocationIdAsync(locId);
        if (!activities.Any())
        {
            return new NotFoundObjectResult(new { message = "No activities found for this location." });
        }

        return new OkObjectResult(activities);
    }
    [Function("GetLatestActivityUserById")]
    public async Task<IActionResult> GetLatestActivityUserById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activity/user/{userId}")] HttpRequest req, string userId)
    {
        if (!int.TryParse(userId, out int userid))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var activity = await _activityService.GetLatestActivityUserByIdAsync(userid);
        if (activity == null)
        {
            return new NotFoundObjectResult(new { message = "Activity not found" });
        }

        return new OkObjectResult(activity);
    }


}
