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


    [Function("GetAllExercises")]
    public async Task<IActionResult> GetAllExercises(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "exercise")] HttpRequest req)
    {
        var exercises = await _exerciseService.GetAllExercisesAsync();
        return new OkObjectResult(exercises);
    }

    [Function("GetExerciseById")]
    public async Task<IActionResult> GetExerciseById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "exercise/get/{id}")] HttpRequest req, string id)

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

}
