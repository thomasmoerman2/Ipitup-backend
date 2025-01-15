using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("TestTrigger function processed a request.");
            return new OkObjectResult(new { Message = "TestTrigger function processed a request." });
        }
    }
}
