namespace Ipitup.Repositories;
public interface IFollowRepository
{
    Task<bool> AddFollowAsync(Follow follow);
    Task<bool> RemoveFollowAsync(int followerId, int followingId);
    Task<bool> AcceptFollowRequestAsync(int followerId, int followingId);
    Task<bool> RemoveFollowerAsync(int followerId, int followingId);
    Task<IEnumerable<Follow>> GetFollowersAsync(int userId);
    Task<IEnumerable<Follow>> GetFollowingAsync(int userId);
    Task<bool> RejectFollowRequestAsync(int followerId, int followingId);
    Task<Follow> CheckIfUserIsFollowingAsync(int followerId, int followingId);
}
public class FollowRepository : IFollowRepository
{
    private readonly string _connectionString;
    public FollowRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }
    public async Task<bool> AddFollowAsync(Follow follow)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("INSERT INTO Follow (followerId, followingId, status, followDate) VALUES (@followerId, @followingId, @status, @followDate)", connection);
        command.Parameters.AddWithValue("@followerId", follow.FollowerId);
        command.Parameters.AddWithValue("@followingId", follow.FollowingId);
        command.Parameters.AddWithValue("@status", follow.Status.ToString());
        command.Parameters.AddWithValue("@followDate", follow.FollowDate);
        var result = await command.ExecuteNonQueryAsync();
        return result > 0;
    }
    public async Task<bool> AcceptFollowRequestAsync(int followerId, int followingId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("UPDATE Follow SET status = 'Accepted' WHERE followerId = @followerId AND followingId = @followingId", connection);
        command.Parameters.AddWithValue("@followerId", followerId);
        command.Parameters.AddWithValue("@followingId", followingId);
        return await command.ExecuteNonQueryAsync() > 0;
    }
    public async Task<bool> RemoveFollowAsync(int followerId, int followingId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("DELETE FROM Follow WHERE followerId = @followerId AND followingId = @followingId", connection);
        command.Parameters.AddWithValue("@followerId", followerId);
        command.Parameters.AddWithValue("@followingId", followingId);
        return await command.ExecuteNonQueryAsync() > 0;
    }
    public async Task<IEnumerable<Follow>> GetFollowersAsync(int userId)
    {
        var followers = new List<Follow>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Follow WHERE followingId = @userId", connection);
        command.Parameters.AddWithValue("@userId", userId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            followers.Add(new Follow
            {
                FollowerId = reader.GetInt32(reader.GetOrdinal("FollowerId")),
                FollowingId = reader.GetInt32(reader.GetOrdinal("FollowingId")),
                Status = Enum.Parse<FollowStatus>(reader.GetString(reader.GetOrdinal("Status"))),
                FollowDate = reader.GetDateTime(reader.GetOrdinal("FollowDate"))
            });
        }
        return followers;
    }
    public async Task<IEnumerable<Follow>> GetFollowingAsync(int userId)
    {
        var following = new List<Follow>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("SELECT * FROM Follow WHERE followerId = @userId", connection);
        command.Parameters.AddWithValue("@userId", userId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            following.Add(new Follow
            {
                FollowerId = reader.GetInt32(reader.GetOrdinal("FollowerId")),
                FollowingId = reader.GetInt32(reader.GetOrdinal("FollowingId")),
                Status = Enum.Parse<FollowStatus>(reader.GetString(reader.GetOrdinal("Status"))),
                FollowDate = reader.GetDateTime(reader.GetOrdinal("FollowDate"))
            });
        }
        return following;
    }
    public async Task<bool> RejectFollowRequestAsync(int followerId, int followingId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("UPDATE Follow SET status = 'Rejected' WHERE followerId = @followerId AND followingId = @followingId", connection);
        command.Parameters.AddWithValue("@followerId", followerId);
        command.Parameters.AddWithValue("@followingId", followingId);
        return await command.ExecuteNonQueryAsync() > 0;
    }
    public async Task<bool> RemoveFollowerAsync(int followerId, int followingId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("DELETE FROM Follow WHERE followerId = @followerId AND followingId = @followingId", connection);
        command.Parameters.AddWithValue("@followerId", followerId);
        command.Parameters.AddWithValue("@followingId", followingId);
        return await command.ExecuteNonQueryAsync() > 0;
    }
    public async Task<Follow> CheckIfUserIsFollowingAsync(int followerId, int followingId)
    {
        try
        {
            Console.WriteLine($"CheckIfUserIsFollowingAsync -> FollowerId: {followerId}, FollowingId: {followingId}");
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT status FROM Follow WHERE followerId = @followerId AND followingId = @followingId", connection);
            command.Parameters.AddWithValue("@followerId", followerId);
            command.Parameters.AddWithValue("@followingId", followingId);
            var result = await command.ExecuteScalarAsync();
            return new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                Status = Enum.Parse<FollowStatus>(result.ToString()),
                FollowDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CheckIfUserIsFollowingAsync -> {ex.Message}");
            return null;
        }
    }
}
