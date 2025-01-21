namespace Ipitup.Functions;

public class ExerciseTrigger
{
    private readonly ILogger<ExerciseTrigger> _logger;
    private readonly IExerciseService _exerciseService;

    public ExerciseTrigger(ILogger<ExerciseTrigger> logger, IExerciseService exerciseService)
    {
        _logger = logger;
        _exerciseService = exerciseService;
    }

    [Function("PostExercise")]
    public async Task<IActionResult> PostExercise(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "exercise/add")] HttpRequest req)
    {
        _logger.LogInformation("PostExercise function triggered");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var exerciseRequest = JsonConvert.DeserializeObject<Exercise>(requestBody);

        if (exerciseRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _exerciseService.AddExerciseAsync(exerciseRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to add exercise" });
        }

        return new OkObjectResult(new { message = "Exercise added successfully" });
    }

    [Function("UpdateExerciseById")]
    public async Task<IActionResult> UpdateExerciseById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "exercise/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int exerciseId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var exerciseRequest = JsonConvert.DeserializeObject<Exercise>(requestBody);

        if (exerciseRequest == null)
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON format" });
        }

        var result = await _exerciseService.UpdateExerciseByIdAsync(exerciseId, exerciseRequest);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to update exercise" });
        }

        return new OkObjectResult(new { message = "Exercise updated successfully" });
    }

    [Function("RemoveExerciseById")]
    public async Task<IActionResult> RemoveExerciseById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "exercise/{id}")] HttpRequest req, string id)
    {
        if (!int.TryParse(id, out int exerciseId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var result = await _exerciseService.DeleteExerciseAsync(exerciseId);
        if (!result)
        {
            return new BadRequestObjectResult(new { message = "Failed to remove exercise" });
        }

        return new OkObjectResult(new { message = "Exercise removed successfully" });
    }

    [Function("GetAllExercises")]
    public async Task<IActionResult> GetAllExercises(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "exercise")] HttpRequest req)
    {
        var exercises = await _exerciseService.GetAllExercisesAsync();
        return new OkObjectResult(exercises);
    }

    [Function("GetExerciseById")]
    public async Task<IActionResult> GetExerciseById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "exercise/{id}")] HttpRequest req, string id)

    {
        if (!int.TryParse(id, out int exerciseId))
        {
            return new BadRequestObjectResult(new { message = "Invalid ID format. It must be a number." });
        }

        var exercise = await _exerciseService.GetExerciseByIdAsync(exerciseId);
        if (exercise == null)
        {
            return new NotFoundObjectResult(new { message = "Exercise not found" });
        }

        return new OkObjectResult(exercise);
    }

    [Function("GetRandomExercise")]
    public async Task<IActionResult> GetRandomExercise(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "exercise/random")] HttpRequest req)

    {
        var exercise = await _exerciseService.GetRandomExerciseAsync();

        if (exercise == null)
        {
            return new NotFoundObjectResult(new { message = "Exercise not found" });
        }

        return new OkObjectResult(exercise);
    }

}
