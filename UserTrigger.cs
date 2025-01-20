using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ipitup.Functions
{
    public class UserTrigger
    {
        private readonly ILogger<UserTrigger> _logger;
        private readonly UserService _userService;

        public UserTrigger(ILogger<UserTrigger> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [Function("UserTrigger")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/login")] HttpRequest req)
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
    }
}
