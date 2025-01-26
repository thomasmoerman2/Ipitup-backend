namespace Ipitup.Services;
public interface INotificationService
{
    Task<List<Notifications>> GetNotificationsAsync(int userId);
    Task UpdateNotificationsAsReadAsync(int userId);
    Task AddAsync(Notifications notification);
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
    public async Task UpdateNotificationsAsReadAsync(int userId)
    {
        await _notificationsRepository.UpdateNotificationsAsReadAsync(userId);
    }
    public async Task AddAsync(Notifications notification)
    {
        await _notificationsRepository.AddAsync(notification);
    }
}
