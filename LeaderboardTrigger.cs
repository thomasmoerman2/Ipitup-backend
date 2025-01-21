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

    [Function("PostLeaderboardEntry")]
    public async Task<IActionResult> PostLeaderboardEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "leaderboard/add")] HttpRequest req)
    {
        _logger.LogInformation("PostLeaderboardEntry function triggered");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var leaderboardRequest = JsonConvert.DeserializeObject<Leaderboard>(requestBody);

        if (leaderboardRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _leaderboardService.AddLeaderboardEntryAsync(leaderboardRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to add leaderboard entry" });
        }

        return new OkObjectResult(new { message = "Leaderboard entry added successfully" });
    }

    [Function("GetLeaderboardById")]
    public async Task<IActionResult> GetLeaderboardById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/{id}")] HttpRequest req, string id)
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
}
