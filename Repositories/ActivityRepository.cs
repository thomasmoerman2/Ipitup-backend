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
    private readonly IBadgeRepository _badgeRepository;
    private readonly IExerciseRepository _exerciseRepository;
    public ActivityRepository(IBadgeRepository badgeRepository, IExerciseRepository exerciseRepository)
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
        _badgeRepository = badgeRepository;
        _exerciseRepository = exerciseRepository;
    }
    public async Task<bool> AddActivityAsync(Activity activity)
    {
        try
        {
            Console.WriteLine($"Adding activity for user {activity.UserId}");
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            Console.WriteLine("Database connection opened");

            var command = new MySqlCommand(@"
                INSERT INTO Activity (userId, activityScore, activityDuration, activityDate, locationId, exerciseId)
                VALUES (@userId, @score, @duration, @date, @location, @exercise)", connection);
            command.Parameters.AddWithValue("@userId", activity.UserId);
            command.Parameters.AddWithValue("@score", activity.ActivityScore);
            command.Parameters.AddWithValue("@duration", activity.ActivityDuration);
            command.Parameters.AddWithValue("@date", activity.ActivityDate);
            command.Parameters.AddWithValue("@location", activity.LocationId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@exercise", activity.ExerciseId ?? (object)DBNull.Value);
            Console.WriteLine("Executing SQL command to add activity");

            var result = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"SQL command executed, result: {result}");

            var badges = await _badgeRepository.GetAllBadgesAsync();
            Console.WriteLine($"Retrieved {badges.Count()} badges");
            var exercise = await _exerciseRepository.GetExerciseByIdAsync(activity.ExerciseId ?? 0);
            Console.WriteLine($"Retrieved exercise with id {exercise.ExerciseId}");

            var badgeUser = await _badgeRepository.GetBadgesByUserIdAsync(activity.UserId);
            Console.WriteLine($"Retrieved {badgeUser.Count()} badges for user {activity.UserId}");

            //filter badges by exerciseId
            var filteredBadges = badges.Where(b => b.BadgeName == exercise.ExerciseName).ToList();
            Console.WriteLine($"Filtered badges, found {filteredBadges.Count} matching badges");

            //loop through filteredBadges and if score is higher than badge score, update badgeUser
            foreach (var badge in filteredBadges)
            {
                if (activity.ActivityScore > badge.BadgeAmount)
                {
                    if (!badgeUser.Any(b => b.BadgeId == badge.BadgeId))
                    {
                        await _badgeRepository.AddBadgeToUserAsync(badge.BadgeId, activity.UserId);
                        Console.WriteLine($"Added badge {badge.BadgeId} to user {activity.UserId}");
                    }
                }
            }

            return result > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding activity: {ex.Message}");
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