namespace Ipitup.Services;
public interface IUserService
{
    Task<bool> RegisterAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> CheckLoginAsync(string email, string password);
}
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<bool> RegisterAsync(User user)
    {
        if (await EmailExistsAsync(user.UserEmail))
            return false;
        return await _userRepository.AddNewUserAsync(user);
    }
    public async Task<bool> CheckLoginAsync(string email, string password)
    {
        return await _userRepository.CheckLoginAsync(email, password);
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
        return await _userRepository.GetByEmailAsync(email);
    }
}