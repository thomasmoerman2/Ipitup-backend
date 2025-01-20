namespace Ipitup.Repositories;
public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> CheckLoginAsync(string email, string password);
    Task<User?> AuthenticateAsync(string email, string password);
    Task<bool> AddNewUserAsync(User user);
    new Task<User?> GetByIdAsync(int id);
    Task<bool> GetByEmailAsync(string email);
}
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationContext context) : base(context) { }
    public async Task<bool> CheckLoginAsync(string email, string password)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email && u.UserPassword == password);
        if (user == null) return false;
        return true;

        // TODO: Uncomment this when the password is hashed
        // return BCrypt.Net.BCrypt.Verify(password, user.UserPassword);
    }
    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email);
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
    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserId == id);
    }
    public async Task<bool> GetByEmailAsync(string email)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email);
        return user != null;
    }
}