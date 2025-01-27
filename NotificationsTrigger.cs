namespace Ipitup.Functions
{
    public class NotificationsTrigger
    {
        private readonly ILogger<NotificationsTrigger> _logger;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        public NotificationsTrigger(ILogger<NotificationsTrigger> logger, INotificationService notificationService, IUserService userService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _userService = userService;
        }
        [Function("NotificationsTrigger")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "notifications/{userId}")] HttpRequest req, string userId)
        {
            var authHeader = req.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new OkResult();
            }
            var token = authHeader.Substring("Bearer ".Length);
            if (!await _userService.VerifyAuthTokenAsync(token))
            {
                return new OkResult();
            }
            if (!int.TryParse(userId, out int userIdInt))
            {
                return new OkResult();
            }
            _logger.LogInformation($"Getting notifications for user {userId} with ID {userIdInt}");
            var notifications = await _notificationService.GetNotificationsAsync(userIdInt);
            return new OkObjectResult(notifications);
        }

        [Function("UpdateNotificationsAsRead")]
        public async Task<IActionResult> UpdateNotificationsAsRead([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "notifications/{userId}/read")] HttpRequest req, string userId)
        {
            try
            {
                _logger.LogInformation($"Starting UpdateNotificationsAsRead for userId: {userId}");

                var authHeader = req.Headers["Authorization"].FirstOrDefault();
                _logger.LogInformation($"Auth header present: {!string.IsNullOrEmpty(authHeader)}");

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Invalid authorization header");
                    return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
                }

                var token = authHeader.Substring("Bearer ".Length);
                _logger.LogInformation("Verifying auth token");

                if (!await _userService.VerifyAuthTokenAsync(token))
                {
                    _logger.LogWarning("Invalid or expired token");
                    return new UnauthorizedObjectResult(new { message = "Invalid or expired token" });
                }

                if (!int.TryParse(userId, out int userIdInt))
                {
                    _logger.LogWarning($"Failed to parse userId: {userId}");
                    return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
                }

                _logger.LogInformation($"Updating notifications as read for user {userIdInt}");
                await _notificationService.UpdateNotificationsAsReadAsync(userIdInt);
                _logger.LogInformation("Successfully updated notifications as read");

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateNotificationsAsRead: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }


        [Function("AddNotification")]
        public async Task<IActionResult> AddNotification([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notifications/{userId}")] HttpRequest req, string userId)
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
            var notification = await new StreamReader(req.Body).ReadToEndAsync();
            var notificationObject = JsonConvert.DeserializeObject<Notifications>(notification);
            if (notificationObject == null)
            {
                return new BadRequestObjectResult(new { message = "Invalid notification format" });
            }
            notificationObject.UserId = userIdInt;
            await _notificationService.AddAsync(notificationObject);
            return new OkResult();
        }
    }
}
