namespace Ipitup.Services;
public interface IUserService
{
    Task<bool> CheckConnection();
    Task<bool> CheckEmailAlreadyExists(string email);
    Task<User> CheckLoginAuth(string email, string password);
    Task<User> AddUser(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByFullNameAsync(string firstname, string lastname);
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

    public async Task<User?> GetUserByFullNameAsync(string firstname, string lastname)
    {
        return await _userRepository.GetUserByFullNameAsync(firstname, lastname);
    }

}