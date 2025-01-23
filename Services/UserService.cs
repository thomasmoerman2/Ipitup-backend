namespace Ipitup.Services;
public interface IUserService
{
    Task<bool> CheckConnection();
    Task<bool> CheckEmailAlreadyExists(string email);
    Task<User> CheckLoginAuth(string email, string password);
    Task<User> AddUser(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<List<User>> GetUserByFullNameAsync(string firstname, string lastname);
    Task<AuthToken?> CreateAuthTokenAsync(int userId);
    Task<bool> VerifyAuthTokenAsync(string token);
    Task<bool> InvalidateAuthTokenAsync(string token);
    Task<string> PasswordResetByUserIdAsync(int userId);
    Task<bool> UpdateUserTotalScoreAsync(int userId, int score);
    Task<bool> UpdateUserIsAdminAsync(int userId, bool isAdmin, string token);
    Task<int> GetUserDailyStreakAsync(int userId);
    Task<bool> UpdateUserAsync(int userId, User user);
    Task<bool> UpdateUserAvatarAsync(int userId, string avatar);
    Task<string?> GetUserAvatarAsync(int userId);

}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> AddUser(User user)
    {
        return await _userRepository.AddUser(user);
    }

    public async Task<bool> CheckConnection()
    {
        return await _userRepository.CheckConnection();
    }

    public async Task<bool> CheckEmailAlreadyExists(string email)
    {
        return await _userRepository.CheckEmailAlreadyExists(email);
    }

    public async Task<User> CheckLoginAuth(string email, string password)
    {
        return await _userRepository.CheckLoginAuth(email, password);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<List<User>> GetUserByFullNameAsync(string firstname, string lastname)
    {
        return await _userRepository.GetUserByFullNameAsync(firstname, lastname);
    }

    public async Task<AuthToken?> CreateAuthTokenAsync(int userId)
    {
        return await _userRepository.CreateAuthTokenAsync(userId);
    }

    public async Task<bool> VerifyAuthTokenAsync(string token)
    {
        return await _userRepository.VerifyAuthTokenAsync(token);
    }

    public async Task<bool> InvalidateAuthTokenAsync(string token)
    {
        return await _userRepository.InvalidateAuthTokenAsync(token);
    }


    public async Task<string> PasswordResetByUserIdAsync(int userId)
    {
        return await _userRepository.PasswordResetByUserIdAsync(userId);
    }

    public async Task<bool> UpdateUserTotalScoreAsync(int userId, int score)
    {
        return await _userRepository.UpdateUserTotalScoreAsync(userId, score);
    }

    public async Task<bool> UpdateUserIsAdminAsync(int userId, bool isAdmin, string token)
    {
        return await _userRepository.UpdateUserIsAdminAsync(userId, isAdmin, token);
    }

    public async Task<int> GetUserDailyStreakAsync(int userId)
    {
        return await _userRepository.GetUserDailyStreakAsync(userId);
    }

    public async Task<bool> UpdateUserAvatarAsync(int userId, string avatar)
    {
        return await _userRepository.UpdateUserAvatarAsync(userId, avatar);
    }


    public async Task<string?> GetUserAvatarAsync(int userId)
    {
        return await _userRepository.GetUserAvatarAsync(userId);
    }


    public async Task<bool> UpdateUserAsync(int userId, User user)
    {
        return await _userRepository.UpdateUserAsync(userId, user);
    }

}
