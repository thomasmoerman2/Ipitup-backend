namespace Ipitup_backend.Services;
public interface IUserService
{
    Task<(User? User, string Token)> AuthenticateAsync(string email, string password);
    Task<bool> RegisterAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> EmailExistsAsync(string email);
}
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    public UserService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }
    public async Task<(User? User, string Token)> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.AuthenticateAsync(email, password);
        if (user == null)
            return (null, string.Empty);
        var token = _jwtService.GenerateToken(user);
        return (user, token);
    }
    public async Task<bool> RegisterAsync(User user)
    {
        if (await EmailExistsAsync(user.UserEmail))
            return false;
        return await _userRepository.AddNewUserAsync(user);
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }
    public async Task<bool> UpdateUserAsync(User user)
    {
        var existingUser = await GetByIdAsync(user.UserId);
        if (existingUser == null)
            return false;
        await _userRepository.UpdateAsync(user);
        return true;
    }
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
            return false;
        await _userRepository.DeleteAsync(userId);
        return true;
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await GetByEmailAsync(email);
        return user != null;
    }
}
