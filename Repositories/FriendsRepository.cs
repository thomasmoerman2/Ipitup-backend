namespace Ipitup.Repositories;
public interface IFriendsRepository

{
    Task<IEnumerable<Friends>> GetUserFriendsAsync(int userId);
    Task<bool> AddFriendAsync(int userId, int friendId);
    Task<bool> AcceptFriendRequestAsync(int userId, int friendId);
    Task<bool> RemoveFriendAsync(int userId, int friendId);
}
public class FriendsRepository : IFriendsRepository
{
    public Task<bool> AcceptFriendRequestAsync(int userId, int friendId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddFriendAsync(int userId, int friendId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Friends>> GetUserFriendsAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFriendAsync(int userId, int friendId)
    {
        throw new NotImplementedException();
    }
}
