namespace Ipitup.Repositories;
public interface IActivityRepository
{
    Task<bool> AddActivityAsync(Activity activity);
    Task<IEnumerable<Activity>> GetAllActivitiesAsync();
    Task<Activity?> GetActivityByIdAsync(int id);
    Task<IEnumerable<Activity>> GetActivitiesByLocationIdAsync(int locationId);
    Task<List<Activity>> GetLatestActivityUserByIdAsync(int userId, int days);
    Task<bool> DeleteActivityAsync(int id);
    Task<bool> UpdateActivityByIdAsync(int id, Activity activity);
    Task<int> GetActivityCountByUserIdAsync(int userId);
}
public class ActivityRepository : IActivityRepository
{
    private readonly string _connectionString;
    public ActivityRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }
    public async Task<bool> AddActivityAsync(Activity activity)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new MySqlCommand(@"
                INSERT INTO Activity (userId, activityScore, activityDuration, activityDate, locationId, exerciseId) 
                VALUES (@userId, @score, @duration, @date, @location, @exercise)", connection);
            command.Parameters.AddWithValue("@userId", activity.UserId);
            command.Parameters.AddWithValue("@score", activity.ActivityScore);
            command.Parameters.AddWithValue("@duration", activity.ActivityDuration);
            command.Parameters.AddWithValue("@date", activity.ActivityDate);
            command.Parameters.AddWithValue("@location", activity.LocationId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@exercise", activity.ExerciseId ?? (object)DBNull.Value);
            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding activity", ex);
        }
    }
    public async Task<int> GetActivityCountByUserIdAsync(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT COUNT(*) FROM Activity WHERE userId = @userId", connection);
        command.Parameters.AddWithValue("@userId", userId);
        command.CommandTimeout = 30;  // Zet een tijdslimiet op de query
        var result = await command.ExecuteNonQueryAsync();
        return result;
    }
    public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        var activities = new List<Activity>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Activity", connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            activities.Add(new Activity
            {
                ActivityId = reader.GetInt32(reader.GetOrdinal("activityId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                ActivityScore = reader.GetInt32(reader.GetOrdinal("activityScore")),
                ActivityDuration = reader.GetInt32(reader.GetOrdinal("activityDuration")),
                ActivityDate = reader.GetDateTime(reader.GetOrdinal("activityDate")),
                LocationId = reader.IsDBNull(reader.GetOrdinal("locationId")) ? null : reader.GetInt32(reader.GetOrdinal("locationId")),
                ExerciseId = reader.IsDBNull(reader.GetOrdinal("exerciseId")) ? null : reader.GetInt32(reader.GetOrdinal("exerciseId"))
            });
        }
        return activities;
    }
    public async Task<Activity?> GetActivityByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Activity WHERE activityId = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Activity
            {
                ActivityId = reader.GetInt32(reader.GetOrdinal("activityId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                ActivityScore = reader.GetInt32(reader.GetOrdinal("activityScore")),
                ActivityDuration = reader.GetInt32(reader.GetOrdinal("activityDuration")),
                ActivityDate = reader.GetDateTime(reader.GetOrdinal("activityDate")),
                LocationId = reader.IsDBNull(reader.GetOrdinal("locationId")) ? null : reader.GetInt32(reader.GetOrdinal("locationId")),
                ExerciseId = reader.IsDBNull(reader.GetOrdinal("exerciseId")) ? null : reader.GetInt32(reader.GetOrdinal("exerciseId"))
            };
        }
        return null;
    }
    public async Task<IEnumerable<Activity>> GetActivitiesByLocationIdAsync(int locationId)
    {
        var activities = new List<Activity>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Activity WHERE locationId = @locationId", connection);
        command.Parameters.AddWithValue("@locationId", locationId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            activities.Add(new Activity
            {
                ActivityId = reader.GetInt32(reader.GetOrdinal("activityId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                ActivityScore = reader.GetInt32(reader.GetOrdinal("activityScore")),
                ActivityDuration = reader.GetInt32(reader.GetOrdinal("activityDuration")),
                ActivityDate = reader.GetDateTime(reader.GetOrdinal("activityDate")),
                LocationId = reader.IsDBNull(reader.GetOrdinal("locationId")) ? null : reader.GetInt32(reader.GetOrdinal("locationId")),
                ExerciseId = reader.IsDBNull(reader.GetOrdinal("exerciseId")) ? null : reader.GetInt32(reader.GetOrdinal("exerciseId"))
            });
        }
        return activities;
    }
    public async Task<List<Activity>> GetLatestActivityUserByIdAsync(int userId, int days)
    {
        var activities = new List<Activity>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Activity WHERE userId = @userId ORDER BY activityDate DESC LIMIT @days", connection);
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@days", days);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            activities.Add(new Activity
            {
                ActivityId = reader.GetInt32(reader.GetOrdinal("activityId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                ActivityScore = reader.GetInt32(reader.GetOrdinal("activityScore")),
                ActivityDuration = reader.GetInt32(reader.GetOrdinal("activityDuration")),
                ActivityDate = reader.GetDateTime(reader.GetOrdinal("activityDate")),
                LocationId = reader.IsDBNull(reader.GetOrdinal("locationId")) ? null : reader.GetInt32(reader.GetOrdinal("locationId")),
                ExerciseId = reader.IsDBNull(reader.GetOrdinal("exerciseId")) ? null : reader.GetInt32(reader.GetOrdinal("exerciseId"))
            });
        }
        return activities;
    }
    public async Task<bool> DeleteActivityAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("DELETE FROM Activity WHERE activityId = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
    public async Task<bool> UpdateActivityByIdAsync(int id, Activity activity)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("UPDATE Activity SET activityScore = @score, activityDuration = @duration, activityDate = @date, locationId = @locationId, exerciseId = @exerciseId WHERE activityId = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@score", activity.ActivityScore);
            command.Parameters.AddWithValue("@duration", activity.ActivityDuration);
            command.Parameters.AddWithValue("@date", activity.ActivityDate);
            command.Parameters.AddWithValue("@locationId", activity.LocationId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@exerciseId", activity.ExerciseId ?? (object)DBNull.Value);
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
