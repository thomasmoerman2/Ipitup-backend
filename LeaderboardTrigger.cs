namespace Ipitup.Functions;

public class LeaderboardTrigger
{
    private readonly ILogger<LeaderboardTrigger> _logger;
    private readonly ILeaderboardService _leaderboardService;
    private readonly IActivityRepository _activityRepository;
    

    public LeaderboardTrigger(ILogger<LeaderboardTrigger> logger, ILeaderboardService leaderboardService, IActivityRepository activityRepository)
    {
        _logger = logger;
        _leaderboardService = leaderboardService;
        _activityRepository = activityRepository;
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

    [Function("GetLeaderboardByLocationId")]
    public async Task<IActionResult> GetLeaderboardByLocationId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/location/{locationId}")] HttpRequest req, string locationId)
    {
        if (!int.TryParse(locationId, out int locId))
        {
            return new BadRequestObjectResult(new { message = "Invalid location ID format. It must be a number." });
        }

        var leaderboardEntries = await _leaderboardService.GetLeaderboardByLocationIdAsync(locId);
        return new OkObjectResult(leaderboardEntries);
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

    if (!string.IsNullOrWhiteSpace(userIdQuery) && int.TryParse(userIdQuery, out int parsedUserId))
    {
        userId = parsedUserId;
    }

    var leaderboardEntries = await _leaderboardService.GetLeaderboardWithFiltersAsync(locationIds, minAge, maxAge, sortType, userId);

    if (!leaderboardEntries.Any())
    {
        return new NotFoundObjectResult(new { message = "No leaderboard entries found with given filters." });
    }

    return new OkObjectResult(leaderboardEntries);
}



}
