namespace Ipitup.Repositories;

public interface IExerciseRepository
{
    Task<bool> AddExerciseAsync(Exercise exercise);
    Task<IEnumerable<Exercise>> GetAllExercisesAsync();
    Task<Exercise?> GetExerciseByIdAsync(int id);
    Task<List<Exercise>> GetRandomExerciseAsync();
    Task<bool> DeleteExerciseAsync(int id);
    Task<bool> UpdateExerciseByIdAsync(int id, Exercise exercise);
    Task<List<Exercise>> GetAllExercisesByCategoriesAsync(List<string> categories);
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
        Console.WriteLine($"Attempting to get exercise with ID: {id}");
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Exercise WHERE exerciseId = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                Console.WriteLine($"Executing query for ID {id}");

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var exercise = new Exercise
                        {
                            ExerciseId = reader.GetInt32(reader.GetOrdinal("exerciseId")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("exerciseName")),
                            ExerciseType = reader.GetString(reader.GetOrdinal("exerciseType")),
                            ExerciseInstructions = reader.IsDBNull(reader.GetOrdinal("exerciseInstructions")) ? null : reader.GetString(reader.GetOrdinal("exerciseInstructions")),
                            ExerciseTime = reader.GetInt32(reader.GetOrdinal("exerciseTime"))
                        };
                        Console.WriteLine($"Found exercise: {exercise.ExerciseName} (ID: {exercise.ExerciseId})");
                        return exercise;
                    }
                    Console.WriteLine($"No exercise found with ID: {id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting exercise with ID {id}: {ex.Message}");
                throw;
            }
        }
        return null;
    }

    public async Task<List<Exercise>> GetRandomExerciseAsync()
    {
        var exercises = new List<Exercise>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();
                // Get 3 random exercises directly from the database
                var command = new MySqlCommand(
                    "SELECT * FROM Exercise ORDER BY RAND() LIMIT 3",
                    connection
                );

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
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting random exercises: {ex.Message}");
                throw;
            }
        }

        return exercises;
    }

    public async Task<bool> DeleteExerciseAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            Console.WriteLine($"Attempting to delete exercise with ID: {id}");
            await connection.OpenAsync();
            var command = new MySqlCommand("DELETE FROM Exercise WHERE exerciseId = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }

    public async Task<bool> UpdateExerciseByIdAsync(int id, Exercise exercise)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("UPDATE Exercise SET exerciseName = @name, exerciseType = @type, exerciseInstructions = @instructions, exerciseTime = @time WHERE exerciseId = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", exercise.ExerciseName);
            command.Parameters.AddWithValue("@type", exercise.ExerciseType);
            command.Parameters.AddWithValue("@instructions", exercise.ExerciseInstructions ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@time", exercise.ExerciseTime);
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }

    public async Task<List<Exercise>> GetAllExercisesByCategoriesAsync(List<string> categories)
    {
        var exercises = new List<Exercise>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Exercise WHERE exerciseType IN @categories", connection);
            command.Parameters.AddWithValue("@categories", categories);
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
}
