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

                var user = await _userService.CheckLoginAsync(userRequest.UserEmail, userRequest.UserPassword);

                if (!user)
                {
                    return new BadRequestObjectResult(new { message = "Invalid credentials" });
                }

                return new OkObjectResult(new
                {
                    message = "UserTrigger worked!",
                    body = new
                    {
                        username = userRequest.UserEmail,
                        password = userRequest.UserPassword
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

                if (userRequest.UserEmail == "" || userRequest.UserPassword == "" || userRequest.UserFirstname == "" || userRequest.UserLastname == "")
                {
                    return new BadRequestObjectResult(new { message = "Invalid request body" });
                }

                var user = await _userService.AddNewUserAsync(userRequest);

                if (!user)
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
    }
}
