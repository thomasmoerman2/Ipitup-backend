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
                var command = new MySqlCommand("SELECT COUNT(*) FROM User WHERE userEmail = @email", connection);
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

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT COUNT(*) FROM User WHERE userEmail = @email AND userPassword = @password", connection);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);
            var result = await command.ExecuteScalarAsync();
            int resultInt = Convert.ToInt32(result);
            return resultInt > 0;
        }
    }
    public async Task<User> AddUser(User user)
    {
        if (!await CheckConnection())
        {
            throw new Exception("Failed to connect to database");
        }

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
}