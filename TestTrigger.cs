namespace Ipitup.Functions
{
    public class TestTrigger
    {
        private readonly ILogger<TestTrigger> _logger;
        public TestTrigger(ILogger<TestTrigger> logger)
        {
            _logger = logger;
        }
        [Function("TestTrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test")] HttpRequest req)
        {
            _logger.LogInformation("TestTrigger function worked");
            return new OkObjectResult(new { message = "TestTrigger worked!" });
        }
        
        [Function("PostLoginTrigger")]
        public async Task<IActionResult> Run2([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test/login")] HttpRequest req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                _logger.LogInformation($"Received request body: {requestBody}");
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult(new { message = "Request body is empty" });
                }
                var loginRequest = JsonConvert.DeserializeObject<User>(requestBody);
                if (loginRequest == null)
                {
                    return new BadRequestObjectResult(new { message = "Invalid JSON format" });
                }
                if (loginRequest.UserEmail == "" || loginRequest.UserPassword == "")
                {
                    return new BadRequestObjectResult(new { message = "Invalid request body" });
                }
                _logger.LogInformation($"Email: {loginRequest.UserEmail}");
                _logger.LogInformation($"Password: {loginRequest.UserPassword}");
                return new OkObjectResult(new
                {
                    message = "PostLoginTrigger worked!",
                    receivedData = new
                    {
                        email = loginRequest.UserEmail,
                        password = loginRequest.UserPassword
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PostLoginTrigger: {ex.Message}");
                return new BadRequestObjectResult(new { message = "Error processing request", error = ex.Message });
            }
        }
        [Function("PostRegisterTrigger")]
        public async Task<IActionResult> Run3([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test/register")] HttpRequest req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var registerRequest = JsonConvert.DeserializeObject<User>(requestBody);
                _logger.LogInformation($"Received request body: {requestBody}");
                _logger.LogInformation("PostRegisterTrigger function worked");
                if (registerRequest == null)
                {
                    return new BadRequestObjectResult(new { message = "Invalid JSON format" });
                }
                if (registerRequest.UserEmail == "" || registerRequest.UserPassword == "")
                {
                    return new BadRequestObjectResult(new { message = "Invalid request body" });
                }
                return new OkObjectResult(new
                {
                    message = "PostRegisterTrigger worked!",
                    body = new
                    {
                        username = registerRequest.UserEmail,
                        password = registerRequest.UserPassword
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PostRegisterTrigger: {ex.Message}");
                return new BadRequestObjectResult(new { message = "Error processing request", error = ex.Message });
            }
        }
        [Function("TestDatabaseConnection")]
        public async Task<IActionResult> RunDbTest([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test/db")] HttpRequest req)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("SQLConnectionOnline");
                await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                await connection.OpenAsync();
                var query = "SELECT 1";
                await using var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection);
                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    _logger.LogInformation("Database connection successful.");
                    return new OkObjectResult(new { message = "Database connection successful" });
                }
                return new BadRequestObjectResult(new { message = "Database connection failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database connection error: {ex.Message}");
                return new BadRequestObjectResult(new { message = "Error connecting to database", error = ex.Message });
            }
        }
    }
}
