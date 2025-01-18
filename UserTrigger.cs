namespace Ipitup_backend.Functions
{
    public class UserTrigger
    {
        private readonly ILogger<UserTrigger> _logger;
        public UserTrigger(ILogger<UserTrigger> logger)
        {
            _logger = logger;
        }
        [Function("UserTrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
