namespace Ipitup.Services;
public interface IFriendsService
{
    Task<IEnumerable<Friends>> GetUserFriendsAsync(int userId);
    Task<bool> AddFriendAsync(int userId, int friendId);
    Task<bool> AcceptFriendRequestAsync(int userId, int friendId);
    Task<bool> RemoveFriendAsync(int userId, int friendId);
}
public class FriendsService : IFriendsService
{
    private readonly IFriendsRepository _friendsRepository;
    public FriendsService(IFriendsRepository friendsRepository)
    {
        _friendsRepository = friendsRepository;
    }
    public async Task<IEnumerable<Friends>> GetUserFriendsAsync(int userId)
    {
        return await _friendsRepository.GetUserFriendsAsync(userId);
    }
    public async Task<bool> AddFriendAsync(int userId, int friendId)
    {
        return await _friendsRepository.AddFriendAsync(userId, friendId);
    }
    public async Task<bool> AcceptFriendRequestAsync(int userId, int friendId)
    {
        return await _friendsRepository.AcceptFriendRequestAsync(userId, friendId);
    }
    public async Task<bool> RemoveFriendAsync(int userId, int friendId)
    {
        return await _friendsRepository.RemoveFriendAsync(userId, friendId);
    }
}
