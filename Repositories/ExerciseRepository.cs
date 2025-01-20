namespace Ipitup.Repositories;

public interface IExerciseRepository
{
    Task<bool> AddExerciseAsync(Exercise exercise);
    Task<IEnumerable<Exercise>> GetAllExercisesAsync();
    Task<Exercise?> GetExerciseByIdAsync(int id);
}

public class ExerciseRepository : IExerciseRepository
{
    private readonly string _connectionString;

    public ExerciseRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }

    public async Task<bool> AddExerciseAsync(Exercise exercise)
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO Exercise (exerciseName, exerciseType, exerciseInstructions, exerciseTime) VALUES (@name, @type, @instructions, @time)", connection);
                command.Parameters.AddWithValue("@name", exercise.ExerciseName);
                command.Parameters.AddWithValue("@type", exercise.ExerciseType);
                command.Parameters.AddWithValue("@instructions", exercise.ExerciseInstructions ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@time", exercise.ExerciseTime);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding exercise", ex);
        }
    }

    public async Task<IEnumerable<Exercise>> GetAllExercisesAsync()
    {
        var exercises = new List<Exercise>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Exercise", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    exercises.Add(new Exercise
                    {
                        ExerciseId = reader.GetInt32(reader.GetOrdinal("exerciseId")),
                        ExerciseName = reader.GetString(reader.GetOrdinal("exerciseName")),
                        ExerciseType = reader.GetString(reader.GetOrdinal("exerciseType")),
                        ExerciseInstructions = reader.IsDBNull(reader.GetOrdinal("exerciseInstructions")) ? null : reader.GetString(reader.GetOrdinal("exerciseInstructions")),
                        ExerciseTime = reader.GetInt32(reader.GetOrdinal("exerciseTime"))

                    });
                }
            }
        }

        return exercises;
    }

    public async Task<Exercise?> GetExerciseByIdAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Exercise WHERE exerciseId = @id", connection);
            command.Parameters.AddWithValue("@id", id);


            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Exercise
                    {
                        ExerciseId = reader.GetInt32(reader.GetOrdinal("exerciseId")),
                        ExerciseName = reader.GetString(reader.GetOrdinal("exerciseName")),
                        ExerciseType = reader.GetString(reader.GetOrdinal("exerciseType")),
                        ExerciseInstructions = reader.IsDBNull(reader.GetOrdinal("exerciseInstructions")) ? null : reader.GetString(reader.GetOrdinal("exerciseInstructions")),
                        ExerciseTime = reader.GetInt32(reader.GetOrdinal("exerciseTime"))
                    };
                }
            }
        }
        return null;
    }
}
