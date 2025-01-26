namespace Ipitup.Repositories;
public interface ILeaderboardRepository
{
    Task<bool> AddLeaderboardEntryAsync(Leaderboard leaderboard);
    Task<Leaderboard?> GetLeaderboardByIdAsync(int leaderboardId);
    Task<IEnumerable<Leaderboard>> GetAllLeaderboardEntriesAsync();
    Task<bool> UpdateLeaderboardScoreAsync(int userId, int locationId, int activityScore);
    Task<IEnumerable<dynamic>> GetLeaderboardWithFiltersAsync(List<int>? locationIds, int? minAge, int? maxAge, string? sortType, int userId);
    Task<int?> GetLeaderboardIdByUserIdAsync(int userId);
    Task<int> GetTotalLocationScoreByUserAsync(int userId, List<int> locationIds);

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
    public async Task<int> GetTotalLocationScoreByUserAsync(int userId, List<int> locationIds)
{
    using var connection = new MySqlConnection(_connectionString);
    await connection.OpenAsync();
    
    var query = $@"
        SELECT SUM(score) AS totalLocationScore
        FROM Leaderboard
        WHERE userId = @userId AND locationId IN ({string.Join(",", locationIds)})";

    using var command = new MySqlCommand(query, connection);
    command.Parameters.AddWithValue("@userId", userId);

    var result = await command.ExecuteScalarAsync();
    return result != DBNull.Value ? Convert.ToInt32(result) : 0;
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
    public async Task<IEnumerable<dynamic>> GetLeaderboardWithFiltersAsync(List<int>? locationIds, int? minAge, int? maxAge, string? sortType, int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        string query = @"
        SELECT 
            u.userId, 
            u.userFirstname, 
            u.userLastname, 
            u.avatar, 
            u.userEmail, 
            u.totalScore, 
            SUM(l.score) AS totalLocationScore, 
            TIMESTAMPDIFF(YEAR, u.birthDate, CURDATE()) AS Age
        FROM Leaderboard l
        INNER JOIN User u ON l.userId = u.userId
        WHERE 1=1";
        var parameters = new List<MySqlParameter>();
        if (locationIds != null && locationIds.Any())
        {
            query += " AND l.locationId IN (" + string.Join(",", locationIds.Select(id => id.ToString())) + ")";
        }
        string ageCondition = "";
        if (minAge.HasValue)
        {
            ageCondition += " AND TIMESTAMPDIFF(YEAR, u.birthDate, CURDATE()) >= @minAge";
            parameters.Add(new MySqlParameter("@minAge", minAge.Value));
        }
        if (maxAge.HasValue)
        {
            ageCondition += " AND TIMESTAMPDIFF(YEAR, u.birthDate, CURDATE()) <= @maxAge";
            parameters.Add(new MySqlParameter("@maxAge", maxAge.Value));
        }
        string sortCondition = "";
        if (sortType == "lokaal")
        {
            sortCondition = " AND u.userCountry = 'Belgium'";
        }
        else if (sortType == "volgend")
        {
            sortCondition = @"
                AND u.userId IN (
                    SELECT followingId FROM Follow WHERE followerId = @userId AND status = 'Accepted'
                )";
            parameters.Add(new MySqlParameter("@userId", userId));
        }
        if (locationIds == null || locationIds.Count == 0)
        {
            // Haal de totale score van alle gebruikers op met leeftijdsfilter en sortering
            query = $@"
                SELECT 
                    u.userId, 
                    u.userFirstname, 
                    u.userLastname, 
                    u.avatar, 
                    u.userEmail, 
                    u.totalScore, 
                    TIMESTAMPDIFF(YEAR, u.birthDate, CURDATE()) AS Age
                FROM User u 
                WHERE 1=1 {ageCondition} {sortCondition}
                ORDER BY u.totalScore DESC 
                LIMIT 10;";
        }
        else
        {
            // Haal leaderboard entries gefilterd op locaties, leeftijd en sortering op
            query = $@"
                SELECT 
                    u.userId, 
                    u.userFirstname, 
                    u.userLastname, 
                    u.avatar, 
                    u.userEmail, 
                    u.totalScore, 
                    SUM(l.score) AS totalLocationScore, 
                    TIMESTAMPDIFF(YEAR, u.birthDate, CURDATE()) AS Age
                FROM Leaderboard l
                INNER JOIN User u ON l.userId = u.userId
                WHERE l.locationId IN ({string.Join(",", locationIds.Select(id => id.ToString()))})
                {ageCondition} {sortCondition}
                GROUP BY u.userId, u.userFirstname, u.userLastname, u.avatar, u.userEmail, u.totalScore, Age
                ORDER BY totalLocationScore DESC
                LIMIT 10;";
        }
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddRange(parameters.ToArray());
        var leaderboardEntries = new List<dynamic>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (locationIds == null || locationIds.Count == 0)
            {
                leaderboardEntries.Add(new
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                    Firstname = reader.GetString(reader.GetOrdinal("userFirstname")),
                    Lastname = reader.GetString(reader.GetOrdinal("userLastname")),
                    Avatar = reader.GetString(reader.GetOrdinal("avatar")),
                    Email = reader.GetString(reader.GetOrdinal("userEmail")),
                    TotalScore = reader.GetInt32(reader.GetOrdinal("totalScore")),
                    Age = reader.GetInt32(reader.GetOrdinal("Age"))
                });
            }
            else
            {
                leaderboardEntries.Add(new
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                    Firstname = reader.GetString(reader.GetOrdinal("userFirstname")),
                    Lastname = reader.GetString(reader.GetOrdinal("userLastname")),
                    Avatar = reader.GetString(reader.GetOrdinal("avatar")),
                    Email = reader.GetString(reader.GetOrdinal("userEmail")),
                    TotalScore = reader.GetInt32(reader.GetOrdinal("totalScore")),
                    TotalLocationScore = reader.GetInt32(reader.GetOrdinal("totalLocationScore")),
                    Age = reader.GetInt32(reader.GetOrdinal("Age"))
                });
            }
        }
        return leaderboardEntries;
    }
    public async Task<int?> GetLeaderboardIdByUserIdAsync(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT LeaderboardId FROM Leaderboard WHERE UserId = @userId", connection);
        command.Parameters.AddWithValue("@userId", userId);
        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt32(result) : (int?)null;
    }
}
