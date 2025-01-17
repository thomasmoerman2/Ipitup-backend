namespace Ipitup_backend.Functions;
public class ActivityTriggers
{
    private readonly IActivityService _activityService;
    private readonly ILogger<ActivityTriggers> _logger;
    public ActivityTriggers(IActivityService activityService, ILogger<ActivityTriggers> logger)
    {
        _activityService = activityService;
        _logger = logger;
    }
    [Function("GetUserActivities")]
    [Ipitup_backend.Attributes.Authorize]
    public async Task<HttpResponseData> GetUserActivities(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/user/{userId}")] HttpRequestData req,
        int userId)
    {
        try
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            var activities = await _activityService.GetByUserIdAsync(userId);
            await response.WriteAsJsonAsync(activities);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserActivities");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { message = "Internal server error" });
            return response;
        }
    }
    [Function("AddActivity")]
    [Authorize]
    public async Task<HttpResponseData> AddActivity(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities")] HttpRequestData req)
    {
        try
        {
            var activity = await req.ReadFromJsonAsync<Activity>();
            if (activity == null)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { message = "Invalid activity data" });
                return badRequest;
            }
            var result = await _activityService.AddActivityAsync(activity);
            var response = req.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new
            {
                message = result ? "Activity added successfully" : "Failed to add activity"
            });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddActivity");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { message = "Internal server error" });
            return response;
        }
    }
    [Function("GetUserTotalScore")]
    [Authorize]
    public async Task<HttpResponseData> GetUserTotalScore(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/user/{userId}/score")] HttpRequestData req,
        int userId)
    {
        try
        {
            var totalScore = await _activityService.GetUserTotalScoreAsync(userId);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { totalScore });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserTotalScore");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { message = "Internal server error" });
            return response;
        }
    }
}
