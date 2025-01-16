namespace Ipitup_backend.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(Context context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email);
    }
}