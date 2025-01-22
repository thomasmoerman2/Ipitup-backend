namespace Ipitup.Functions;

public class LocationTrigger
{
    private readonly ILogger<LocationTrigger> _logger;
    private readonly ILocationService _locationService;

    public LocationTrigger(ILogger<LocationTrigger> logger, ILocationService locationService)
    {
        _logger = logger;
        _locationService = locationService;
    }

    [Function("PostLocation")]
    public async Task<IActionResult> PostLocation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "location/add")] HttpRequest req)
    {
        _logger.LogInformation("PostLocation function triggered");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var locationRequest = JsonConvert.DeserializeObject<Location>(requestBody);

        if (locationRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _locationService.AddLocationAsync(locationRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to add location" });
        }

        return new OkObjectResult(new { message = "Location added successfully" });
    }

    [Function("UpdateLocationById")]
    public async Task<IActionResult> UpdateLocationById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "location/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int locationId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var locationRequest = JsonConvert.DeserializeObject<Location>(requestBody);

        if (locationRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _locationService.UpdateLocationByIdAsync(locationId, locationRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to update location" });
        }

        return new OkObjectResult(new { message = "Location updated successfully" });
    }

    [Function("RemoveLocationById")]
    public async Task<IActionResult> RemoveLocationById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "location/remove")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var locationRequest = JsonConvert.DeserializeObject<Location>(requestBody);

        if (locationRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _locationService.DeleteLocationAsync(locationRequest.LocationId);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to remove location" });
        }

        return new OkObjectResult(new { message = "Location removed successfully" });
    }

    [Function("GetAllLocations")]
    public async Task<IActionResult> GetAllLocations(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "location")] HttpRequest req)
    {
        var locations = await _locationService.GetAllLocationsAsync();
        return new OkObjectResult(locations);
    }

    [Function("GetLocationById")]
    public async Task<IActionResult> GetLocationById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "location/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int locationId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var location = await _locationService.GetLocationByIdAsync(locationId);
        if (location == null)
        {
            return new NotFoundObjectResult(new { message = "Location not found" });
        }

        return new OkObjectResult(location);
    }
}
