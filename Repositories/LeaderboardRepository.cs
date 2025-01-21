namespace Ipitup.Repositories;

public interface ILeaderboardRepository
{
    Task<bool> AddLeaderboardEntryAsync(Leaderboard leaderboard);
    Task<Leaderboard?> GetLeaderboardByIdAsync(int leaderboardId);
    Task<IEnumerable<Leaderboard>> GetLeaderboardByLocationIdAsync(int locationId);
    Task<IEnumerable<Leaderboard>> GetAllLeaderboardEntriesAsync();
    Task<bool> UpdateLeaderboardScoreAsync(int userId, int locationId, int activityScore);
}

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly string _connectionString;

    public LeaderboardRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }

    public async Task<bool> AddLeaderboardEntryAsync(Leaderboard leaderboard)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand(@"
            INSERT INTO Leaderboard (userId, locationId, score)
            VALUES (@userId, @locationId, @score)", connection);

        command.Parameters.AddWithValue("@userId", leaderboard.UserId);
        command.Parameters.AddWithValue("@locationId", leaderboard.LocationId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@score", leaderboard.Score);

        var result = await command.ExecuteNonQueryAsync();
        return result > 0;
    }

    public async Task<Leaderboard?> GetLeaderboardByIdAsync(int leaderboardId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Leaderboard WHERE leaderboardId = @id", connection);
        command.Parameters.AddWithValue("@id", leaderboardId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Leaderboard
            {
                LeaderboardId = reader.GetInt32(reader.GetOrdinal("leaderboardId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                LocationId = reader.IsDBNull(reader.GetOrdinal("locationId")) ? null : reader.GetInt32(reader.GetOrdinal("locationId")),
                Score = reader.GetInt32(reader.GetOrdinal("score"))
            };
        }
        return null;
    }

    public async Task<IEnumerable<Leaderboard>> GetLeaderboardByLocationIdAsync(int locationId)
    {
        var leaderboardEntries = new List<Leaderboard>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Leaderboard WHERE locationId = @locationId", connection);
        command.Parameters.AddWithValue("@locationId", locationId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            leaderboardEntries.Add(new Leaderboard
            {
                LeaderboardId = reader.GetInt32(reader.GetOrdinal("leaderboardId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                LocationId = reader.IsDBNull(reader.GetOrdinal("locationId")) ? null : reader.GetInt32(reader.GetOrdinal("locationId")),
                Score = reader.GetInt32(reader.GetOrdinal("score"))
            });
        }
        return leaderboardEntries;
    }

    public async Task<IEnumerable<Leaderboard>> GetAllLeaderboardEntriesAsync()
    {
        var leaderboardEntries = new List<Leaderboard>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Leaderboard", connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            leaderboardEntries.Add(new Leaderboard
            {
                LeaderboardId = reader.GetInt32(reader.GetOrdinal("leaderboardId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                LocationId = reader.IsDBNull(reader.GetOrdinal("locationId")) ? null : reader.GetInt32(reader.GetOrdinal("locationId")),
                Score = reader.GetInt32(reader.GetOrdinal("score"))
            });
        }
        return leaderboardEntries;
    }

    public async Task<bool> UpdateLeaderboardScoreAsync(int userId, int locationId, int activityScore)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // Check if the leaderboard entry exists
        var checkCommand = new MySqlCommand(@"
            SELECT COUNT(*) FROM Leaderboard WHERE UserId = @userId AND LocationId = @locationId;", connection);
        checkCommand.Parameters.AddWithValue("@userId", userId);
        checkCommand.Parameters.AddWithValue("@locationId", locationId);

        var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

        if (count == 0)
        {
            // Insert new leaderboard entry if none exists
            var insertCommand = new MySqlCommand(@"
                INSERT INTO Leaderboard (UserId, LocationId, Score)
                VALUES (@userId, @locationId, @activityScore);", connection);

            insertCommand.Parameters.AddWithValue("@userId", userId);
            insertCommand.Parameters.AddWithValue("@locationId", locationId);
            insertCommand.Parameters.AddWithValue("@activityScore", activityScore);

            var insertResult = await insertCommand.ExecuteNonQueryAsync();
            return insertResult > 0;
        }
        else
        {
            // Update the leaderboard score if entry exists
            var updateCommand = new MySqlCommand(@"
                UPDATE Leaderboard
                SET Score = Score + @activityScore
                WHERE UserId = @userId AND LocationId = @locationId;", connection);

            updateCommand.Parameters.AddWithValue("@userId", userId);
            updateCommand.Parameters.AddWithValue("@locationId", locationId);
            updateCommand.Parameters.AddWithValue("@activityScore", activityScore);

            var updateResult = await updateCommand.ExecuteNonQueryAsync();
            return updateResult > 0;
        }
    }


}
