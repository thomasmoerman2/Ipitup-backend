namespace Ipitup.Services;

public interface IFollowService
{

    Task<bool> FollowUserAsync(Follow follow);

    Task<IEnumerable<Follow>> GetFollowersAsync(int userId);

    Task<bool> AcceptFollowRequestAsync(int followerId, int followingId);

    Task<bool> UnfollowUserAsync(int followerId, int followingId);

    Task<bool> RejectFollowRequestAsync(int followerId, int followingId);
    Task<bool> RemoveFollowerAsync(int followerId, int followingId);


}

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;

    public FollowService(IFollowRepository followRepository)
    {
        _followRepository = followRepository;
    }

    public async Task<bool> FollowUserAsync(Follow follow)
    {
        return await _followRepository.AddFollowAsync(follow);
    }

    public async Task<bool> AcceptFollowRequestAsync(int followerId, int followingId)
    {
        return await _followRepository.AcceptFollowRequestAsync(followerId, followingId);
    }

    public async Task<bool> UnfollowUserAsync(int followerId, int followingId)
    {
        return await _followRepository.RemoveFollowAsync(followerId, followingId);
    }

    public async Task<IEnumerable<Follow>> GetFollowersAsync(int userId)
    {
        return await _followRepository.GetFollowersAsync(userId);
    }

    public async Task<IEnumerable<Follow>> GetFollowingAsync(int userId)
    {
        return await _followRepository.GetFollowingAsync(userId);
    }


    public async Task<bool> RejectFollowRequestAsync(int followerId, int followingId)
    {
        return await _followRepository.RejectFollowRequestAsync(followerId, followingId);
    }

    public async Task<bool> RemoveFollowerAsync(int followerId, int followingId)
    {
        return await _followRepository.RemoveFollowerAsync(followerId, followingId);
    }

}
