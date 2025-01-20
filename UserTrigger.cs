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
        [Function("PostUserLogin")]
        public async Task<IActionResult> PostUserLogin([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/login")] HttpRequest req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var userRequest = JsonConvert.DeserializeObject<User>(requestBody);
                _logger.LogInformation($"Received request body: {requestBody}");
                _logger.LogInformation("UserTrigger function worked");

                if (userRequest == null)
                {
                    return new BadRequestObjectResult(new { message = "Invalid JSON format" });
                }
                if (userRequest.UserEmail == "" || userRequest.UserPassword == "")
                {
                    return new BadRequestObjectResult(new { message = "Invalid request body" });
                }
                var user = await _userService.CheckLoginAuth(userRequest.UserEmail, userRequest.UserPassword);
                if (user == null)
                {
                    return new BadRequestObjectResult(new { message = "Invalid credentials" });
                }
                return new OkObjectResult(new
                {
                    status = 200,
                    body = new
                    {
                        userId = user.UserId,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UserTrigger: {ex.Message}");
                return new BadRequestObjectResult(new { message = "Error processing request", error = ex.Message });
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
                if (userRequest.UserEmail == "" || userRequest.UserPassword == "" || userRequest.UserFirstname == "" || userRequest.UserLastname == "" || userRequest.Avatar == "" || userRequest.BirthDate == DateTime.MinValue)
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
                return new OkObjectResult(new { message = "UserTrigger worked!", body = userRequest });
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

    }
}
