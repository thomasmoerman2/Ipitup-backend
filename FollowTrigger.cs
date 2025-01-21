namespace Ipitup.Functions;

public class FollowTrigger
{
    private readonly ILogger<FollowTrigger> _logger;
    private readonly IFollowService _followService;

    public FollowTrigger(ILogger<FollowTrigger> logger, IFollowService followService)
    {
        _logger = logger;
        _followService = followService;
    }

    [Function("FollowUser")]
    public async Task<IActionResult> FollowUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "follow")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var follow = JsonConvert.DeserializeObject<Follow>(requestBody);
        if (follow == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid request data" });
        }

        var result = await _followService.FollowUserAsync(follow);
        return result ? new OkObjectResult(new { message = "Follow request sent" }) : new BadRequestObjectResult(new { message = "Failed to follow user" });
    }

    [Function("GetFollowers")]
    public async Task<IActionResult> GetFollowers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "followers/{userId}")] HttpRequest req, int userId)
    {
        var followers = await _followService.GetFollowersAsync(userId);
        return new OkObjectResult(followers);
    }

    [Function("AcceptFollowRequest")]
    public async Task<IActionResult> AcceptFollowRequest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "follow/accept")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var follow = JsonConvert.DeserializeObject<Follow>(requestBody);
        if (follow == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid request data" });
        }

        var result = await _followService.AcceptFollowRequestAsync(follow.FollowerId, follow.FollowingId);
        return result ? new OkObjectResult(new { message = "Follow request accepted" }) : new BadRequestObjectResult(new { message = "Failed to accept follow request" });
    }

    [Function("UnfollowUser")]
    public async Task<IActionResult> UnfollowUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "follow/unfollow")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var follow = JsonConvert.DeserializeObject<Follow>(requestBody);
        if (follow == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid request data" });
        }

        var result = await _followService.UnfollowUserAsync(follow.FollowerId, follow.FollowingId);
        return result ? new OkObjectResult(new { message = "User unfollowed successfully" }) : new BadRequestObjectResult(new { message = "Failed to unfollow user" });
    }

}
