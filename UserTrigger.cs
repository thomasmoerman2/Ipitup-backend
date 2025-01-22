using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ipitup.Functions
{
    public class UserTrigger
    {
        private readonly ILogger<UserTrigger> _logger;
        private readonly IUserService _userService;

        public UserTrigger(ILogger<UserTrigger> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
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
                    authToken = authToken.Token
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
            _logger.LogInformation("=====>>>> VerifyToken function called");
            try
            {
                string? authHeader = req.Headers["Authorization"].FirstOrDefault();
                _logger.LogInformation($"AuthHeader: {authHeader}");
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return new UnauthorizedObjectResult(new { message = "Invalid authorization header" });
                }
                _logger.LogInformation($"AuthHeader: {authHeader}");

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
                    _logger.LogError($"Invalid request body: UserEmail={userRequest.UserEmail}, UserPassword={userRequest.UserPassword}, UserFirstname={userRequest.UserFirstname}, UserLastname={userRequest.UserLastname}, BirthDate={userRequest.BirthDate}. One or more of these values are empty or invalid.");
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
                    return new OkObjectResult(new { message = "UserTrigger worked!", body = userRequest, authToken = authToken.Token });
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

            return new OkObjectResult(user);
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/fullname")] HttpRequest req)
        {
            string? firstname = req.Query["firstname"];
            string? lastname = req.Query["lastname"];

            if (string.IsNullOrWhiteSpace(firstname) || string.IsNullOrWhiteSpace(lastname))
            {
                return new BadRequestObjectResult(new { message = "Firstname and Lastname are required" });
            }

            var user = await _userService.GetUserByFullNameAsync(firstname, lastname);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found" });
            }

            return new OkObjectResult(user);
        }

        [Function("GetUserTotalScore")]
        public async Task<IActionResult> GetUserTotalScore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/totalscore/{id}")] HttpRequest req, string id)
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

            return new OkObjectResult(new { totalScore = user.TotalScore });
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

    }
}

