namespace Ipitup.Functions
{
    public class UserTrigger
    {
        private readonly ILogger<UserTrigger> _logger;
        private readonly IUserService _userService;
        private readonly IActivityService _activityService;
        private readonly IBadgeService _badgeService;
        private readonly IExerciseService _exerciseService;
        private readonly IFollowService _followService;
        private readonly INotificationService _notificationService;
        public UserTrigger(ILogger<UserTrigger> logger, IUserService userService, IActivityService activityService, IBadgeService badgeService, IExerciseService exerciseService, IFollowService followService, INotificationService notificationService)
        {
            _logger = logger;
            _userService = userService;
            _activityService = activityService;
            _badgeService = badgeService;
            _exerciseService = exerciseService;
            _followService = followService;
            _notificationService = notificationService;
        }
        [Function("Login")]
        public async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/login")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var loginRequest = System.Text.Json.JsonSerializer.Deserialize<LoginRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    return new BadRequestObjectResult(new { message = "Email and password are required" });
                }
                var user = await _userService.CheckLoginAuth(loginRequest.Email, loginRequest.Password);
                if (user == null)
                {
                    return new UnauthorizedObjectResult(new { message = "Invalid email or password" });
                }
                // Create auth token
                var authToken = await _userService.CreateAuthTokenAsync(user.UserId);
                if (authToken == null)
                {
                    return new BadRequestObjectResult(new { message = "Failed to create auth token" });
                }
                return new OkObjectResult(new
                {
                    userId = user.UserId,
                    firstname = user.UserFirstname,
                    lastname = user.UserLastname,
                    email = user.UserEmail,
                    accountStatus = user.AccountStatus,
                    authToken = authToken.Token,
                    isAdmin = user.IsAdmin
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        private class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
        [Function("VerifyToken")]
        public async Task<IActionResult> VerifyToken([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "verify")] HttpRequest req)
        {
            try
            {
                string? authHeader = req.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
                }
                string token = authHeader.Substring("Bearer ".Length);
                bool isValid = await _userService.VerifyAuthTokenAsync(token);
                if (!isValid)
                {
                    return new UnauthorizedObjectResult(new { message = "Invalid or expired token" });
                }
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        [Function("Logout")]
        public async Task<IActionResult> Logout([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/logout")] HttpRequest req)
        {
            try
            {
                string? authHeader = req.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
                }
                string token = authHeader.Substring("Bearer ".Length);
                bool result = await _userService.InvalidateAuthTokenAsync(token);
                if (!result)
                {
                    return new BadRequestObjectResult(new { message = "Failed to logout" });
                }
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        [Function("PostUserRegister")]
        public async Task<IActionResult> PostUserRegister([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/register")] HttpRequest req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var userRequest = JsonConvert.DeserializeObject<User>(requestBody);
                if (userRequest == null)
                {
                    return new BadRequestObjectResult(new { message = "Invalid JSON format" });
                }
                if (userRequest.UserEmail == "" || userRequest.UserPassword == "" || userRequest.UserFirstname == "" || userRequest.UserLastname == "" || userRequest.BirthDate == DateTime.MinValue)
                {
                    return new BadRequestObjectResult(new { message = "Invalid request body" });
                }
                var emailExists = await _userService.CheckEmailAlreadyExists(userRequest.UserEmail);
                if (emailExists)
                {
                    return new BadRequestObjectResult(new { message = "User already exists" });
                }
                var user = await _userService.AddUser(userRequest);
                if (user == null)
                {
                    return new BadRequestObjectResult(new { message = "User already exists" });
                }
                if (user.UserId > 0)
                {
                    var authToken = await _userService.CreateAuthTokenAsync(user.UserId);
                    if (authToken == null)
                    {
                        return new BadRequestObjectResult(new { message = "Failed to create auth token" });
                    }


                    // log the user data
                    _logger.LogInformation($"User created: {user.UserId}, {user.UserFirstname}, {user.UserLastname}, {user.UserEmail}, {user.Avatar}, {user.BirthDate}, {user.AccountStatus}, {user.DailyStreak}, {user.TotalScore}");

                    return new OkObjectResult(new
                    {
                        message = "UserTrigger worked!",
                        body = new
                        {
                            userId = user.UserId,
                            firstname = user.UserFirstname,
                            lastname = user.UserLastname,
                            email = user.UserEmail,
                            avatar = user.Avatar,
                            birthDate = user.BirthDate,
                            accountStatus = user.AccountStatus,
                            dailyStreak = user.DailyStreak,
                            totalScore = user.TotalScore,
                            isAdmin = user.IsAdmin
                        },
                        authToken = authToken.Token
                    });
                }
                else
                {
                    return new BadRequestObjectResult(new { message = "User created, failed to generate token" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UserTrigger: {ex.Message}");
                return new BadRequestObjectResult(new { message = "Error processing request", error = ex.Message });
            }
        }
        [Function("GetUserById")]
        public async Task<IActionResult> GetUserById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{id}")] HttpRequest req, string id)
        {
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found" });
            }
            return new OkObjectResult(new { userId = user.UserId, firstname = user.UserFirstname, lastname = user.UserLastname, email = user.UserEmail, accountStatus = user.AccountStatus, dailyStreak = user.DailyStreak, totalScore = user.TotalScore, isAdmin = user.IsAdmin });
        }
        [Function("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequest req)
        {
            var users = await _userService.GetAllUsersAsync();
            return new OkObjectResult(users);
        }
        [Function("GetUserByFullName")]
        public async Task<IActionResult> GetUserByFullName(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/fullname")] HttpRequest req)
        {
            string firstname = req.Query["firstname"].ToString() ?? string.Empty;
            string lastname = req.Query["lastname"].ToString() ?? string.Empty;
            var user = await _userService.GetUserByFullNameAsync(firstname, lastname);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found" });
            }
            return new OkObjectResult(new
            {
                status = 200,
                message = "User found",
                body = user.Select(u => new
                {
                    id = u.UserId,
                    firstname = u.UserFirstname,
                    lastname = u.UserLastname,
                    avatar = u.Avatar,
                    dailyStreak = u.DailyStreak,
                    totalScore = u.TotalScore
                })
            });
        }
        [Function("GetUserTotalScore")]
        public async Task<IActionResult> GetUserTotalScore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/totalscore/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation($"GetUserTotalScore function triggered with ID: '{id}'");
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError("ID is null or empty");
                    return new BadRequestObjectResult(new { message = "ID is required" });
                }

                int userId = int.Parse(id);

                if (userId <= 0)
                {
                    return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError($"User not found with ID: {userId}");
                    return new NotFoundObjectResult(new { message = "User not found" });
                }

                _logger.LogInformation($"User found with ID: {userId}. Total score: {user.TotalScore}");
                return new OkObjectResult(new { totalScore = user.TotalScore });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUserTotalScore. ID: '{id}', Error: {ex.Message}", ex);
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
        }
        [Function("PasswordResetByUserId")]
        public async Task<IActionResult> PasswordResetByUserId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/reset-password")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = System.Text.Json.JsonSerializer.Deserialize<PasswordResetRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (request == null || request.UserId <= 0)
                {
                    return new BadRequestObjectResult(new { message = "Invalid request. User ID is required." });
                }
                var result = await _userService.PasswordResetByUserIdAsync(request.UserId);
                if (string.IsNullOrEmpty(result))
                {
                    return new BadRequestObjectResult(new { message = "Failed to reset password" });
                }
                return new OkObjectResult(new
                {
                    message = "Password reset successfully",
                    newPassword = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        [Function("UpdateUserIsAdmin")]
        public async Task<IActionResult> UpdateUserIsAdmin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/admin")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var userRequest = JsonConvert.DeserializeObject<User>(requestBody);
            if (userRequest == null)
            {
                return new BadRequestObjectResult(new { message = "Invalid JSON format" });
            }
            var authHeader = req.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
            }
            var result = await _userService.UpdateUserIsAdminAsync(userRequest.UserId, userRequest.IsAdmin, authHeader.Substring("Bearer ".Length));
            if (!result)
            {
                return new BadRequestObjectResult(new { message = "Failed to update user is admin" });
            }
            return new OkObjectResult(new { message = "User is admin updated successfully" });
        }
        private class PasswordResetRequest
        {
            public int UserId { get; set; }
        }
        [Function("GetUserDailyStreak")]
        public async Task<IActionResult> GetUserDailyStreak(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/dailystreak/{id}")] HttpRequest req, string id)
        {
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found" });
            }
            return new OkObjectResult(new { dailyStreak = user.DailyStreak });
        }
        [Function("UpdateUserAvatar")]
        public async Task<IActionResult> UpdateUserAvatar(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/avatar/{id}")] HttpRequest req, string id)
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
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var avatarData = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);
            if (avatarData == null || !avatarData.ContainsKey("avatar"))
            {
                return new BadRequestObjectResult(new { message = "Invalid JSON format" });
            }
            var result = await _userService.UpdateUserAvatarAsync(userId, avatarData["avatar"]);
            if (!result)
            {
                return new BadRequestObjectResult(new { message = "Failed to update user avatar" });
            }
            return new OkObjectResult(new { message = "User avatar updated successfully" });
        }
        [Function("UpdateUser")]
        public async Task<IActionResult> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/{id}")] HttpRequest req, string id)
        {
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }
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
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var userRequest = JsonConvert.DeserializeObject<User>(requestBody);
            if (userRequest == null)
            {
                return new BadRequestObjectResult(new { message = "Invalid JSON format" });
            }
            var result = await _userService.UpdateUserAsync(userId, userRequest);
            if (!result)
            {
                return new BadRequestObjectResult(new { message = "Failed to update user" });
            }
            return new OkObjectResult(new { message = "User updated successfully" });
        }
        [Function("GetUserAvatar")]
        public async Task<IActionResult> GetUserAvatar(
                 [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/avatar/{id}")] HttpRequest req, string id)
        {
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }
            var avatar = await _userService.GetUserAvatarAsync(userId);
            return new OkObjectResult(new { avatar });
        }
        [Function("GetUserByIdLimited")]
        public async Task<IActionResult> GetUserByIdLimited(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/info/{id}")] HttpRequest req, string id)
        {
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found" });
            }

            var activities = await _activityService.GetLatestActivityUserByIdAsync(userId, 30) ?? new List<Activity>();
            var latestActivities = activities.Take(3).ToList();
            var exerciseIds = latestActivities.Select(a => a.ExerciseId).ToList();

            var exercises = await _exerciseService.GetExercisesByIdsAsync(exerciseIds) ?? new List<Exercise>();
            var latestExercises = exercises.Take(3).ToList();

            Follow? follow = null;
            var authHeader = req.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);
                var userIdFromToken = await _userService.GetUserIdFromTokenAsync(token);
                if (userIdFromToken > 0)
                {
                    follow = await _followService.CheckIfUserIsFollowingAsync(userIdFromToken, userId);
                }
            }

            var achievements = await _badgeService.GetLatestBadgesByUserIdAsync(userId, 8) ?? new List<Badge>();
            var latestAchievements = achievements.Take(8).ToList();

            var exercisesObject = latestActivities.Select(a => new
            {
                name = exercises.FirstOrDefault(e => e.ExerciseId == a.ExerciseId)?.ExerciseName ?? "Unknown",
                type = exercises.FirstOrDefault(e => e.ExerciseId == a.ExerciseId)?.ExerciseType ?? "Unknown",
                time = exercises.FirstOrDefault(e => e.ExerciseId == a.ExerciseId)?.ExerciseTime ?? 0,
                score = a.ActivityScore
            }).ToList();

            var achievementsObject = latestAchievements.Select(a => new
            {
                id = a.BadgeId,
                name = a.BadgeName,
                description = a.BadgeDescription,
                amount = a.BadgeAmount
            }).ToList();

            if (follow != null && follow.Status == FollowStatus.Accepted)
            {
                return new OkObjectResult(new
                {
                    userId = user.UserId,
                    isFollowing = true,
                    firstname = user.UserFirstname,
                    lastname = user.UserLastname,
                    avatar = user.Avatar,
                    exercises = exercisesObject,
                    accountStatus = user.AccountStatus,
                    achievements = achievementsObject,
                    leaderboard = new { score = user.TotalScore }
                });
            }
            else if (user.AccountStatus == AccountStatus.Public)
            {
                return new OkObjectResult(new
                {
                    userId = user.UserId,
                    isFollowing = follow?.Status == FollowStatus.Accepted,
                    firstname = user.UserFirstname,
                    lastname = user.UserLastname,
                    avatar = user.Avatar,
                    exercises = exercisesObject,
                    accountStatus = user.AccountStatus,
                    achievements = achievementsObject,
                    leaderboard = new { score = user.TotalScore }
                });
            }
            else
            {
                return new OkObjectResult(new
                {
                    userId = user.UserId,
                    accountStatus = user.AccountStatus,
                    firstname = user.UserFirstname,
                    lastname = user.UserLastname,
                    avatar = user.Avatar ?? string.Empty,
                    dailyStreak = user.DailyStreak, 
                    isPending = follow?.Status == FollowStatus.Pending
                });
            }
        }



        [Function("UpdateAccountStatus")]
        public async Task<IActionResult> UpdateAccountStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/accountstatus")] HttpRequest req)
        {
            var authHeader = req.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
            }

            var token = authHeader.Substring("Bearer ".Length);
            var userId = await _userService.GetUserIdFromTokenAsync(token);
            if (userId == 0)
            {
                return new UnauthorizedObjectResult(new { message = "Invalid or expired token" });
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);

            if (request == null || !request.ContainsKey("accountStatus"))
            {
                return new BadRequestObjectResult(new { message = "Invalid JSON format" });
            }

            var newStatus = Enum.TryParse<AccountStatus>(request["accountStatus"], out var accountStatus)
                ? accountStatus
                : AccountStatus.Private;

            var result = await _userService.UpdateUserAccountStatusAsync(userId, newStatus);

            if (!result)
            {
                return new BadRequestObjectResult(new { message = "Failed to update account status" });
            }

            return new OkObjectResult(new { message = "Account status updated successfully", status = newStatus });
        }


        [Function("GetListOfFollowingByUserId")]
        public async Task<IActionResult> GetListOfFollowingByUserId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{id}/following")] HttpRequest req, string id)
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
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }
            var following = await _followService.GetFollowingAsync(userId);
            var formattedFollowing = new List<object>();
            foreach (var follow in following)
            {
                var user = await _userService.GetUserByIdAsync(follow.FollowingId);
                formattedFollowing.Add(new
                {
                    id = user.UserId,
                    firstname = user.UserFirstname,
                    lastname = user.UserLastname,
                    avatar = user.Avatar
                });
            }
            return new OkObjectResult(formattedFollowing);
        }
        [Function("GetListOfFollowersByUserId")]
        public async Task<IActionResult> GetListOfFollowersByUserId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{id}/followers")] HttpRequest req, string id)
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
            if (!int.TryParse(id, out int userId))
            {
                return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
            }
            var followers = await _followService.GetFollowersAsync(userId);
            var formattedFollowers = new List<object>();
            foreach (var follower in followers)
            {
                var user = await _userService.GetUserByIdAsync(follower.FollowerId);
                formattedFollowers.Add(new
                {
                    id = user.UserId,
                    firstname = user.UserFirstname,
                    lastname = user.UserLastname,
                    avatar = user.Avatar,
                    isFollowing = follower.Status == FollowStatus.Accepted
                });
            }
            return new OkObjectResult(formattedFollowers);
        }
    }
}
