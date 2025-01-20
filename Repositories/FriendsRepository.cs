namespace Ipitup_backend.Repositories;
public interface IFriendsRepository : IGenericRepository<Friends>
{
    Task<IEnumerable<Friends>> GetUserFriendsAsync(int userId);
    Task<bool> AddFriendAsync(int userId, int friendId);
    Task<bool> AcceptFriendRequestAsync(int userId, int friendId);
    Task<bool> RemoveFriendAsync(int userId, int friendId);
}
public class FriendsRepository : GenericRepository<Friends>, IFriendsRepository
{
    public FriendsRepository(ApplicationContext context) : base(context) { }
    public async Task<IEnumerable<Friends>> GetUserFriendsAsync(int userId)
    {
        return await _dbSet
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == FriendStatus.Accepted)
            .ToListAsync();
    }
    public async Task<bool> AddFriendAsync(int userId, int friendId)
    {
        var friendship = new Friends
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendStatus.Waiting
        };
        var result = await AddAsync(friendship);
        return result != null;
    }
    public async Task<bool> AcceptFriendRequestAsync(int userId, int friendId)
    {
        var friendship = await _dbSet
            .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId);
        if (friendship == null)
            return false;
        friendship.Status = FriendStatus.Accepted;
        await UpdateAsync(friendship);
        return true;
    }
    public async Task<bool> RemoveFriendAsync(int userId, int friendId)
    {
        var friendship = await _dbSet
            .FirstOrDefaultAsync(f =>
                (f.UserId == userId && f.FriendId == friendId) ||
                (f.UserId == friendId && f.FriendId == userId));
        if (friendship == null)
            return false;
        _dbSet.Remove(friendship);
        await _context.SaveChangesAsync();
        return true;
    }
}
