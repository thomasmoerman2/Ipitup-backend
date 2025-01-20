namespace Ipitup.Repositories;
public interface IUserRepository
{
    Task<bool> CheckConnection();
    Task<bool> CheckEmailAlreadyExists(string email);
    Task<bool> CheckLoginAuth(string email, string password);
    Task<User> AddUser(User user);
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
                var command = new MySqlCommand($"SELECT COUNT(*) FROM users WHERE email = @email", connection);
                command.Parameters.AddWithValue("@email", email);
                var result = await command.ExecuteScalarAsync();
                return result != null && (int)result > 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to check email existence", ex);
        }
    }
    public async Task<bool> CheckLoginAuth(string email, string password)
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand($"SELECT COUNT(*) FROM users WHERE email = @email AND password = @password", connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                var result = await command.ExecuteScalarAsync();
                return result != null && (int)result > 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to authenticate login", ex);
        }
    }
    public async Task<User> AddUser(User user)
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand($"INSERT INTO users (email, password) VALUES (@email, @password) RETURNING *", connection);
                command.Parameters.AddWithValue("@email", user.UserEmail);
                command.Parameters.AddWithValue("@password", user.UserPassword);
                var result = await command.ExecuteScalarAsync();
                return result != null ? (User)result : null;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add user", ex);
        }
    }
}