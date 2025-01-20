namespace Ipitup.Services;

public interface IExerciseService
{
    Task<bool> AddExerciseAsync(Exercise exercise);
    Task<IEnumerable<Exercise>> GetAllExercisesAsync();
    Task<Exercise?> GetExerciseByIdAsync(int id);
    Task<List<Exercise>> GetRandomExerciseAsync();
}

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;

    public ExerciseService(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<bool> AddExerciseAsync(Exercise exercise)
    {
        if (string.IsNullOrWhiteSpace(exercise.ExerciseName) ||
            string.IsNullOrWhiteSpace(exercise.ExerciseType) ||
            exercise.ExerciseTime < 0)
        {
            throw new ArgumentException("Invalid exercise data");
        }

        return await _exerciseRepository.AddExerciseAsync(exercise);
    }

    public async Task<IEnumerable<Exercise>> GetAllExercisesAsync()
    {
        return await _exerciseRepository.GetAllExercisesAsync();
    }

    public async Task<Exercise?> GetExerciseByIdAsync(int id)
    {
        return await _exerciseRepository.GetExerciseByIdAsync(id);
    }

    public async Task<List<Exercise>> GetRandomExerciseAsync()
    {
        return await _exerciseRepository.GetRandomExerciseAsync();
    }
}
