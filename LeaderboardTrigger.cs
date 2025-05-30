namespace Ipitup.Functions;
public class LeaderboardTrigger
{
    private readonly ILogger<LeaderboardTrigger> _logger;
    private readonly ILeaderboardService _leaderboardService;
    private readonly IActivityRepository _activityRepository;
    private readonly IUserService _userService;

    public LeaderboardTrigger(ILogger<LeaderboardTrigger> logger, ILeaderboardService leaderboardService, IActivityRepository activityRepository, IUserService userService)
    {
        _logger = logger;
        _leaderboardService = leaderboardService;
        _activityRepository = activityRepository;
        _userService = userService;
    }
    [Function("GetLeaderboardById")]
    public async Task<IActionResult> GetLeaderboardById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/byid/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int leaderboardId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }
        var leaderboard = await _leaderboardService.GetLeaderboardByIdAsync(leaderboardId);
        if (leaderboard == null)
        {
            return new NotFoundObjectResult(new { message = "Leaderboard entry not found" });
        }
        return new OkObjectResult(leaderboard);
    }

    [Function("GetLeaderboardByLocationIds")]
    public async Task<IActionResult> GetLeaderboardByLocationIds(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/location/{locationIds}/user/{userId}")] HttpRequest req, string locationIds, string userId)
    {
        if (!int.TryParse(userId, out int userIdInt))
        {
            return new BadRequestObjectResult(new { message = "Invalid user ID format." });
        }

        var locationIdList = locationIds.Split(',')
            .Select(id => int.TryParse(id, out int locId) ? locId : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();

        if (!locationIdList.Any())
        {
            return new BadRequestObjectResult(new { message = "Invalid location IDs provided." });
        }

        var totalLocationScore = await _leaderboardService.GetTotalLocationScoreByUserAsync(userIdInt, locationIdList);
        var user = await _userService.GetUserByIdAsync(userIdInt);

        if (user == null)
        {
            return new NotFoundObjectResult(new { message = "User not found" });
        }

        return new OkObjectResult(new 
        { 
            userId = userIdInt, 
            firstname = user.UserFirstname, 
            lastname = user.UserLastname, 
            totalLocationScore 
        });
    }



    [Function("GetAllLeaderboardEntries")]
    public async Task<IActionResult> GetAllLeaderboardEntries(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard")] HttpRequest req)
    {
        var leaderboardEntries = await _leaderboardService.GetAllLeaderboardEntriesAsync();
        return new OkObjectResult(leaderboardEntries);
    }
    [Function("GetLeaderboardWithFilters")]
    public async Task<IActionResult> GetLeaderboardWithFilters(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/filter")] HttpRequest req)
    {
        var locationIdQuery = req.Query["locationIds"].ToString();
        var minAgeQuery = req.Query["minAge"];
        var maxAgeQuery = req.Query["maxAge"];
        var sortType = req.Query["sortType"].ToString();
        var userIdQuery = req.Query["userId"];
        List<int>? locationIds = null;
        int? minAge = null;
        int? maxAge = null;
        int userId = 0;
        if (!string.IsNullOrWhiteSpace(locationIdQuery))
        {
            locationIds = locationIdQuery
                .Split(',')
                .Select(id => int.TryParse(id, out int locId) ? locId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();
        }
        if (!string.IsNullOrWhiteSpace(minAgeQuery) && int.TryParse(minAgeQuery, out int min))
        {
            minAge = min;
        }
        if (!string.IsNullOrWhiteSpace(maxAgeQuery) && int.TryParse(maxAgeQuery, out int max))
        {
            maxAge = max;
        }
        if (sortType == "volgend")
        {
            if (!string.IsNullOrWhiteSpace(userIdQuery) && int.TryParse(userIdQuery, out int parsedUserId))
            {
                userId = parsedUserId;
            }
            else
            {
                return new BadRequestObjectResult(new { message = "Invalid userId provided for 'volgend' sortType." });
            }
        }
        _logger.LogInformation("Fetching leaderboard with sortType: {0}, userId: {1}", sortType, userId);
        _logger.LogInformation("Received parameters: locationIds={0}, minAge={1}, maxAge={2}", 
                                locationIds != null ? string.Join(",", locationIds) : "None", 
                                minAge?.ToString() ?? "None", 
                                maxAge?.ToString() ?? "None");
        var leaderboardEntries = await _leaderboardService.GetLeaderboardWithFiltersAsync(locationIds, minAge, maxAge, sortType, userId);
        if (!leaderboardEntries.Any())
        {
            return new NotFoundObjectResult(new { message = "No leaderboard entries found with given filters." });
        }
        return new OkObjectResult(leaderboardEntries);
    }
    [Function("GetLeaderboardByUserId")]
    public async Task<IActionResult> GetLeaderboardByUserId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/byuserid/{userId}")] HttpRequest req, string userId)
    {
        if (!int.TryParse(userId, out int userIdInt))
        {
            return new BadRequestObjectResult(new { message = "Invalid user ID format." });
        }
        var leaderboardId = await _leaderboardService.GetLeaderboardIdByUserIdAsync(userIdInt);
        if (leaderboardId == null)
        {
            return new NotFoundObjectResult(new { message = "Leaderboard entry not found for user." });
        }
        return new OkObjectResult(new { leaderboardId });
    }

    
}
