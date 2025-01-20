namespace Ipitup.Services;
public interface IUserService
{
    Task<bool> CheckConnection();
    Task<bool> CheckEmailAlreadyExists(string email);
    Task<bool> CheckLoginAuth(string email, string password);
    Task<User> AddUser(User user);
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

    public async Task<bool> CheckLoginAuth(string email, string password)
    {
        return await _userRepository.CheckLoginAuth(email, password);
    }
}