namespace Ipitup.Functions;

public class BadgeTrigger
{
    private readonly ILogger<BadgeTrigger> _logger;
    private readonly IBadgeService _badgeService;

    public BadgeTrigger(ILogger<BadgeTrigger> logger, IBadgeService badgeService)
    {
        _logger = logger;
        _badgeService = badgeService;
    }

    [Function("PostBadge")]
    public async Task<IActionResult> PostBadge(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "badge/add")] HttpRequest req)
    {
        _logger.LogInformation("PostBadge function triggered");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var badgeRequest = JsonConvert.DeserializeObject<Badge>(requestBody);

        if (badgeRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _badgeService.AddBadgeAsync(badgeRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to add badge" });
        }

        return new OkObjectResult(new { message = "Badge added successfully" });
    }

    [Function("UpdateBadgeById")]
    public async Task<IActionResult> UpdateBadgeById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "badge/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int badgeId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var badgeRequest = JsonConvert.DeserializeObject<Badge>(requestBody);

        if (badgeRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _badgeService.UpdateBadgeByIdAsync(badgeId, badgeRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to update badge" });
        }

        return new OkObjectResult(new { message = "Badge updated successfully" });
    }

    [Function("RemoveBadgeById")]
    public async Task<IActionResult> RemoveBadgeById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "badge/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int badgeId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var result = await _badgeService.DeleteBadgeAsync(badgeId);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to remove badge" });
        }

        return new OkObjectResult(new { message = "Badge removed successfully" });
    }

    [Function("GetAllBadges")]
    public async Task<IActionResult> GetAllBadges(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "badge")] HttpRequest req)
    {
        var badges = await _badgeService.GetAllBadgesAsync();
        return new OkObjectResult(badges);
    }

    [Function("GetBadgeById")]
    public async Task<IActionResult> GetBadgeById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "badge/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int badgeId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var badge = await _badgeService.GetBadgeByIdAsync(badgeId);
        if (badge == null)
        {
            return new NotFoundObjectResult(new { message = "Badge not found" });
        }

        return new OkObjectResult(badge);
    }

    [Function("AddBadgeToUser")]
    public async Task<IActionResult> AddBadgeToUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "badge/user/add")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(requestBody);

        if (data == null || data.badgeId == null || data.userId == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid request body" });
        }

        int badgeId = data.badgeId;
        int userId = data.userId;

        var result = await _badgeService.AddBadgeToUserAsync(badgeId, userId);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to assign badge to user" });
        }

        return new OkObjectResult(new { message = "Badge assigned to user successfully" });
    }
    
    [Function("GetBadgesByUserId")]
    public async Task<IActionResult> GetBadgesByUserId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "badge/user/{userId}")] HttpRequest req, string userId)
    {
        if (!int.TryParse(userId, out int userIdParsed))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var badges = await _badgeService.GetBadgesByUserIdAsync(userIdParsed);
        return new OkObjectResult(badges);
    }
}
