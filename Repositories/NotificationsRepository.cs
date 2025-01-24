namespace Ipitup.Repositories;
public interface INotificationsRepository
{
    Task<List<Notifications>> GetNotificationsAsync(int userId);
    Task UpdateNotificationsAsReadAsync(int userId, List<int> notificationsIds);
    Task AddNotificationAsync(Notifications notification);
}
public class NotificationsRepository : INotificationsRepository
{
    private readonly string _connectionString;
    public NotificationsRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }
    public async Task<List<Notifications>> GetNotificationsAsync(int userId)
    {
        var notifications = new List<Notifications>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Notifications WHERE userId = @userId AND isRead = 0 ORDER BY notificationId DESC", connection);
            command.Parameters.AddWithValue("@userId", userId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {

                    notifications.Add(new Notifications
                    {
                        NotificationId = reader.GetInt32(reader.GetOrdinal("notificationId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                        Message = reader.GetString(reader.GetOrdinal("message")),
                        IsRead = reader.GetBoolean(reader.GetOrdinal("isRead")),
                        Type = Enum.Parse<NotificationType>(reader.GetString(reader.GetOrdinal("type")))
                    });
                }
            }
        }
        return notifications;
    }
    public async Task UpdateNotificationsAsReadAsync(int userId, List<int> notificationsIds)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("UPDATE Notifications SET isRead = 1 WHERE userId = @userId", connection);
        command.Parameters.AddWithValue("@userId", userId);
        for (int i = 0; i < notificationsIds.Count; i++)
        {
            command.CommandText += $" AND notificationId = @notificationId{i}";
            command.Parameters.AddWithValue($"@notificationId{i}", notificationsIds[i]);
        }
        await command.ExecuteNonQueryAsync();
    }
    public async Task AddNotificationAsync(Notifications notification)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new MySqlCommand("INSERT INTO Notifications (userId, message, isRead, type) VALUES (@userId, @message, @isRead, @type)", connection);
        command.Parameters.AddWithValue("@userId", notification.UserId);
        command.Parameters.AddWithValue("@message", notification.Message);
        command.Parameters.AddWithValue("@isRead", notification.IsRead);
        command.Parameters.AddWithValue("@type", notification.Type);
        await command.ExecuteNonQueryAsync();
    }
}