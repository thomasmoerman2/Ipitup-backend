namespace Ipitup.Repositories;
using System.Security.Cryptography;
using System.Text;

public interface IUserRepository
{
    Task<bool> CheckConnection();
    Task<bool> CheckEmailAlreadyExists(string email);
    Task<User> CheckLoginAuth(string email, string password);
    Task<User> AddUser(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByFullNameAsync(string firstname, string lastname);
    Task<AuthToken?> CreateAuthTokenAsync(int userId);
    Task<AuthToken?> GetAuthTokenAsync(string token);
    Task<bool> InvalidateAuthTokenAsync(string token);
    Task<bool> VerifyAuthTokenAsync(string token);
    Task<string> PasswordResetByUserIdAsync(int userId);
}

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }

    public async Task<bool> CheckConnection()
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> CheckEmailAlreadyExists(string email)
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT COUNT(*) FROM User WHERE userEmail = @email", connection);
                command.Parameters.AddWithValue("@email", email);
                var result = await command.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    return count > 0;
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Database query error in CheckEmailAlreadyExists: {ex.Message}", ex);
        }
    }

    public async Task<User> CheckLoginAuth(string email, string password)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM User WHERE userEmail = @email", connection);
            command.Parameters.AddWithValue("@email", email);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    var storedHashedPassword = reader.GetString(reader.GetOrdinal("userPassword"));
                    if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                            UserEmail = reader.GetString(reader.GetOrdinal("userEmail")),
                            UserFirstname = reader.GetString(reader.GetOrdinal("userFirstname")),
                            UserLastname = reader.GetString(reader.GetOrdinal("userLastname")),
                            Avatar = reader.GetString(reader.GetOrdinal("avatar")),
                            BirthDate = reader.GetDateTime(reader.GetOrdinal("birthDate"))
                        };
                    }
                }
            }
        }
        return null; // Return null als de verificatie mislukt
    }

    public async Task<User> AddUser(User user)
    {
        if (!await CheckConnection())
        {
            throw new Exception("Failed to connect to database");
        }

        // Hash het wachtwoord voordat het wordt opgeslagen
        user.UserPassword = BCrypt.Net.BCrypt.HashPassword(user.UserPassword);

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("INSERT INTO User (userEmail, userPassword, userFirstname, userLastname, avatar, birthDate) VALUES (@email, @password, @firstname, @lastname, @avatar, @birthdate)", connection);
            command.Parameters.AddWithValue("@email", user.UserEmail);
            command.Parameters.AddWithValue("@password", user.UserPassword);
            command.Parameters.AddWithValue("@firstname", user.UserFirstname);
            command.Parameters.AddWithValue("@lastname", user.UserLastname);
            command.Parameters.AddWithValue("@avatar", user.Avatar);
            command.Parameters.AddWithValue("@birthdate", user.BirthDate);
            var result = await command.ExecuteNonQueryAsync();
            if (result > 0)
            {
                return user;
            }
            else
            {
                throw new Exception("Failed to add user");
            }
        }
    }


    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = new List<User>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM User", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                        UserEmail = reader.GetString(reader.GetOrdinal("userEmail")),
                        UserFirstname = reader.GetString(reader.GetOrdinal("userFirstname")),
                        UserLastname = reader.GetString(reader.GetOrdinal("userLastname")),
                        Avatar = reader.GetString(reader.GetOrdinal("avatar")),
                        BirthDate = reader.GetDateTime(reader.GetOrdinal("birthDate")),
                        AccountStatus = (AccountStatus)Enum.Parse(typeof(AccountStatus), reader.GetString(reader.GetOrdinal("accountStatus"))),
                        DailyStreak = reader.GetInt32(reader.GetOrdinal("dailyStreak")),
                        TotalScore = reader.GetInt32(reader.GetOrdinal("totalScore"))
                    });
                }
            }
        }
        return users;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM User WHERE userId = @userId", connection);
            command.Parameters.AddWithValue("@userId", userId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                        UserEmail = reader.GetString(reader.GetOrdinal("userEmail")),
                        UserFirstname = reader.GetString(reader.GetOrdinal("userFirstname")),
                        UserLastname = reader.GetString(reader.GetOrdinal("userLastname")),
                        Avatar = reader.GetString(reader.GetOrdinal("avatar")),
                        BirthDate = reader.GetDateTime(reader.GetOrdinal("birthDate")),
                        AccountStatus = (AccountStatus)Enum.Parse(typeof(AccountStatus), reader.GetString(reader.GetOrdinal("accountStatus"))),
                        DailyStreak = reader.GetInt32(reader.GetOrdinal("dailyStreak")),
                        TotalScore = reader.GetInt32(reader.GetOrdinal("totalScore"))
                    };
                }
            }
        }
        return null;
    }

    public async Task<User?> GetUserByFullNameAsync(string firstname, string lastname)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM User WHERE userFirstname = @firstname AND userLastname = @lastname", connection);
            command.Parameters.AddWithValue("@firstname", firstname);
            command.Parameters.AddWithValue("@lastname", lastname);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                        UserEmail = reader.GetString(reader.GetOrdinal("userEmail")),
                        UserFirstname = reader.GetString(reader.GetOrdinal("userFirstname")),
                        UserLastname = reader.GetString(reader.GetOrdinal("userLastname")),
                        Avatar = reader.GetString(reader.GetOrdinal("avatar")),
                        BirthDate = reader.GetDateTime(reader.GetOrdinal("birthDate")),
                        AccountStatus = (AccountStatus)Enum.Parse(typeof(AccountStatus), reader.GetString(reader.GetOrdinal("accountStatus"))),
                        DailyStreak = reader.GetInt32(reader.GetOrdinal("dailyStreak")),
                        TotalScore = reader.GetInt32(reader.GetOrdinal("totalScore"))
                    };
                }
            }
        }
        return null;
    }

    public async Task<AuthToken?> CreateAuthTokenAsync(int userId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Generate a secure random token
            var token = GenerateSecureToken();
            var createdAt = DateTime.UtcNow;
            var expiresAt = createdAt.AddDays(7); // Token expires in 7 days

            var command = new MySqlCommand(
                @"INSERT INTO AuthToken (userId, token, createdAt, expiresAt, isValid) 
                VALUES (@userId, @token, @createdAt, @expiresAt, true)",
                connection);

            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@token", token);
            command.Parameters.AddWithValue("@createdAt", createdAt);
            command.Parameters.AddWithValue("@expiresAt", expiresAt);

            await command.ExecuteNonQueryAsync();

            return new AuthToken
            {
                UserId = userId,
                Token = token,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt,
                IsValid = true
            };
        }
    }

    public async Task<AuthToken?> GetAuthTokenAsync(string token)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand(
                "SELECT * FROM AuthToken WHERE token = @token",
                connection);
            command.Parameters.AddWithValue("@token", token);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new AuthToken
                    {
                        TokenId = reader.GetInt32(reader.GetOrdinal("tokenId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                        Token = reader.GetString(reader.GetOrdinal("token")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                        ExpiresAt = reader.GetDateTime(reader.GetOrdinal("expiresAt")),
                        IsValid = reader.GetBoolean(reader.GetOrdinal("isValid"))
                    };
                }
            }
        }
        return null;
    }

    public async Task<bool> InvalidateAuthTokenAsync(string token)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand(
                "UPDATE AuthToken SET isValid = false WHERE token = @token",
                connection);
            command.Parameters.AddWithValue("@token", token);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }

    public async Task<bool> VerifyAuthTokenAsync(string token)
    {
        var authToken = await GetAuthTokenAsync(token);
        if (authToken == null) return false;

        return authToken.IsValid && authToken.ExpiresAt > DateTime.UtcNow;
    }

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<string> PasswordResetByUserIdAsync(int userId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("UPDATE User SET userPassword = @password WHERE userId = @userId", connection);
            var newPassword = GenerateSecureToken();
            command.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(newPassword));
            command.Parameters.AddWithValue("@userId", userId);
            await command.ExecuteNonQueryAsync();
            return newPassword;
        }
    }
}
