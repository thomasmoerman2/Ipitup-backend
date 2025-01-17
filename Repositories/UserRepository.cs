namespace Ipitup_backend.Repositories;
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> CheckLoginAsync(string email, string password);
    Task<User?> AuthenticateAsync(string email, string password);
    Task<bool> AddNewUserAsync(User user);
    Task<User?> GetByIdAsync(int id);
}
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationContext context) : base(context) { }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email);
    }
    public async Task<bool> CheckLoginAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);
        if (user == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, user.UserPassword);
    }
    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);
        if (user == null) return null;
        if (BCrypt.Net.BCrypt.Verify(password, user.UserPassword))
            return user;
        return null;
    }
    public override async Task<User> AddAsync(User user)
    {
        user.UserPassword = BCrypt.Net.BCrypt.HashPassword(user.UserPassword);
        return await base.AddAsync(user);
    }
    public async Task<bool> AddNewUserAsync(User user)
    {
        user.UserPassword = BCrypt.Net.BCrypt.HashPassword(user.UserPassword);
        var result = await AddAsync(user);
        return result != null;
    }
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserId == id);
    }
}
