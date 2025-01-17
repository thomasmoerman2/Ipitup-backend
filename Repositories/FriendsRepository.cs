namespace Ipitup_backend.Repositories;
public interface IFriendsRepository : IGenericRepository<Friends>
{
    Task<IEnumerable<Friends>> GetByUserIdAsync(int userId);
    Task<bool> AddFriendRequestAsync(Friends friend);
    Task<bool> AddFriendAsync(Friends friend);
}
public class FriendsRepository : GenericRepository<Friends>, IFriendsRepository
{
    public FriendsRepository(ApplicationContext context) : base(context) { }
    public async Task<IEnumerable<Friends>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(f => f.UserId == userId).ToListAsync();
    }
    public async Task<bool> AddFriendRequestAsync(Friends friend)
    {
        var result = await _context.Friends.AddAsync(friend);
        return result != null;
    }
    public async Task<bool> AddFriendAsync(Friends friend)
    {
        var result = await _context.Friends.AddAsync(friend);
        return result != null;
    }
}
