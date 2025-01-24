namespace Ipitup.Services;
public interface INotificationService
{
    Task<List<Notifications>> GetNotificationsAsync(int userId);
    Task UpdateNotificationsAsReadAsync(int userId, List<int> notificationsIds);
    Task AddNotificationAsync(Notifications notification);
}
public class NotificationsService : INotificationService
{
    private readonly INotificationsRepository _notificationsRepository;
    public NotificationsService(INotificationsRepository notificationsRepository)
    {
        _notificationsRepository = notificationsRepository;
    }
    public async Task<List<Notifications>> GetNotificationsAsync(int userId)
    {
        return await _notificationsRepository.GetNotificationsAsync(userId);
    }
    public async Task UpdateNotificationsAsReadAsync(int userId, List<int> notificationsIds)
    {
        await _notificationsRepository.UpdateNotificationsAsReadAsync(userId, notificationsIds);
    }
    public async Task AddNotificationAsync(Notifications notification)
    {
        await _notificationsRepository.AddNotificationAsync(notification);
    }
}
