namespace Ipitup.Functions;
public class FollowTrigger
{
    private readonly ILogger<FollowTrigger> _logger;
    private readonly IFollowService _followService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    public FollowTrigger(ILogger<FollowTrigger> logger, IFollowService followService, IUserService userService, INotificationService notificationService)
    {
        _logger = logger;
        _followService = followService;
        _userService = userService;
        _notificationService = notificationService;
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
        User? user = null;
        if (result)
        {
            user = await _userService.GetUserByIdAsync(follow.FollowerId);
        }
        if (follow.Status == FollowStatus.Pending)
        {
            await _notificationService.AddAsync(new Notifications
            {
                UserId = follow.FollowingId,
                Message = $"{user?.UserFirstname} {user?.UserLastname} vraagt om jou te volgen",
                IsRead = false,
                Type = NotificationType.FriendRequest
            });
        }
        else if (result)
        {
            await _notificationService.AddAsync(new Notifications
            {
                UserId = follow.FollowingId,
                Message = $"{user?.UserFirstname} {user?.UserLastname} volgt je nu",
                IsRead = false,
                Type = NotificationType.FriendRequest
            });
        }

        return result ? new OkObjectResult(new { message = "Follow request sent" }) : new BadRequestObjectResult(new { message = "Failed to follow user" });
    }
    [Function("GetFollowers")]
    public async Task<IActionResult> GetFollowers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "followers/{userId}")] HttpRequest req, string userId)
    {
        var authHeader = req.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
        }
        var token = authHeader.Substring("Bearer ".Length);
        if (!await _userService.VerifyAuthTokenAsync(token))
        {
            return new UnauthorizedObjectResult(new { message = "Invalid or expired token" });
        }
        if (!int.TryParse(userId, out int userIdInt))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }
        var followers = await _followService.GetFollowersAsync(userIdInt);
        return new OkObjectResult(new { followers = followers });
    }
    [Function("GetFollowing")]
    public async Task<IActionResult> GetFollowing(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "following/{userId}")] HttpRequest req, string userId)
    {
        var authHeader = req.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
        }
        var token = authHeader.Substring("Bearer ".Length);
        if (!await _userService.VerifyAuthTokenAsync(token))
        {
            return new UnauthorizedObjectResult(new { message = "Invalid or expired token" });
        }
        if (!int.TryParse(userId, out int userIdInt))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }
        var following = await _followService.GetFollowingAsync(userIdInt);
        return new OkObjectResult(new { following = following });
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "unfollow")] HttpRequest req)
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
    [Function("RejectFollowRequest")]
    public async Task<IActionResult> RejectFollowRequest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "follow/reject")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var follow = JsonConvert.DeserializeObject<Follow>(requestBody);
        if (follow == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid request data" });
        }
        var result = await _followService.RejectFollowRequestAsync(follow.FollowerId, follow.FollowingId);
        return result ? new OkObjectResult(new { message = "Follow request rejected" }) : new BadRequestObjectResult(new { message = "Failed to reject follow request" });
    }
    [Function("RemoveFollower")]
    public async Task<IActionResult> RemoveFollower(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "follow/remove")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var follow = JsonConvert.DeserializeObject<Follow>(requestBody);
        if (follow == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid request data" });
        }
        var result = await _followService.RemoveFollowerAsync(follow.FollowerId, follow.FollowingId);
        return result ? new OkObjectResult(new { message = "Follower removed successfully" }) : new BadRequestObjectResult(new { message = "Failed to remove follower" });
    }
    [Function("CheckIfUserIsFollowing")]
    public async Task<IActionResult> CheckIfUserIsFollowing(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "follow/check")] HttpRequest req)
    {
        var authHeader = req.Headers["Authorization"].FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            return new BadRequestObjectResult(new { message = "Invalid token" });
        }
        var token = authHeader.Substring("Bearer ".Length);
        var userIdFromToken = await _userService.VerifyAuthTokenAsync(token);
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var follow = JsonConvert.DeserializeObject<Follow>(requestBody);
        if (follow == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid request data" });
        }
        var result = await _followService.CheckIfUserIsFollowingAsync(follow.FollowerId, follow.FollowingId);
        return new OkObjectResult(new { isFollowing = result });
    }
}
