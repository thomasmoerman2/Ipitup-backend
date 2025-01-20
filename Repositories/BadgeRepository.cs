namespace Ipitup.Repositories;

public interface IBadgeRepository
{
    Task<bool> AddBadgeAsync(Badge badge);
    Task<IEnumerable<Badge>> GetAllBadgesAsync();
    Task<Badge?> GetBadgeByIdAsync(int id);
    Task<IEnumerable<Badge>> GetBadgesByUserIdAsync(int userId);
    Task<bool> AddBadgeToUserAsync(int badgeId, int userId);
}

public class BadgeRepository : IBadgeRepository
{
    private readonly string _connectionString;

    public BadgeRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }

    public async Task<bool> AddBadgeAsync(Badge badge)
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "INSERT INTO Badge (badgeName, badgeDescription, badgeAmount) VALUES (@name, @description, @amount)", 
                    connection
                );
                command.Parameters.AddWithValue("@name", badge.BadgeName);
                command.Parameters.AddWithValue("@description", badge.BadgeDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@amount", badge.BadgeAmount);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding badge", ex);
        }
    }

    public async Task<IEnumerable<Badge>> GetAllBadgesAsync()
    {
        var badges = new List<Badge>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Badge", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    badges.Add(new Badge
                    {
                        BadgeId = reader.GetInt32(reader.GetOrdinal("badgeId")),
                        BadgeName = reader.GetString(reader.GetOrdinal("badgeName")),
                        BadgeDescription = reader.IsDBNull(reader.GetOrdinal("badgeDescription")) ? null : reader.GetString(reader.GetOrdinal("badgeDescription")),
                        BadgeAmount = reader.GetInt32(reader.GetOrdinal("badgeAmount"))
                    });
                }
            }
        }

        return badges;
    }

    public async Task<Badge?> GetBadgeByIdAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Badge WHERE badgeId = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Badge
                    {
                        BadgeId = reader.GetInt32(reader.GetOrdinal("badgeId")),
                        BadgeName = reader.GetString(reader.GetOrdinal("badgeName")),
                        BadgeDescription = reader.IsDBNull(reader.GetOrdinal("badgeDescription")) ? null : reader.GetString(reader.GetOrdinal("badgeDescription")),
                        BadgeAmount = reader.GetInt32(reader.GetOrdinal("badgeAmount"))
                    };
                }
            }
        }
        return null;
    }

    public async Task<IEnumerable<Badge>> GetBadgesByUserIdAsync(int userId)
    {
        var badges = new List<Badge>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand(
                "SELECT b.* FROM BadgeUser bu INNER JOIN Badge b ON bu.badgeId = b.badgeId WHERE bu.userId = @userId", 
                connection
            );
            command.Parameters.AddWithValue("@userId", userId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    badges.Add(new Badge
                    {
                        BadgeId = reader.GetInt32(reader.GetOrdinal("badgeId")),
                        BadgeName = reader.GetString(reader.GetOrdinal("badgeName")),
                        BadgeDescription = reader.IsDBNull(reader.GetOrdinal("badgeDescription")) ? null : reader.GetString(reader.GetOrdinal("badgeDescription")),
                        BadgeAmount = reader.GetInt32(reader.GetOrdinal("badgeAmount"))
                    });
                }
            }
        }

        return badges;
    }

    public async Task<bool> AddBadgeToUserAsync(int badgeId, int userId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("INSERT INTO BadgeUser (badgeId, userId) VALUES (@badgeId, @userId)", connection);
            command.Parameters.AddWithValue("@badgeId", badgeId);
            command.Parameters.AddWithValue("@userId", userId);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }
    }
}
