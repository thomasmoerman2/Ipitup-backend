namespace Ipitup.Repositories;
public interface IUserRepository
{
    Task<bool> CheckConnection();
    Task<bool> CheckEmailAlreadyExists(string email);
    Task<User> CheckLoginAuth(string email, string password);
    Task<User> AddUser(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByFullNameAsync(string firstname, string lastname);
}
public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    public UserRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString");
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

    public async Task<User?> CheckLoginAuth(string email, string password)
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
}