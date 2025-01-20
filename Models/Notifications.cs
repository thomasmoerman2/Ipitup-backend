namespace Ipitup.Models;
[Table("Notifications")]
public class Notifications
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    [Required]
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public NotificationType Type { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
public enum NotificationType
{
    Alert,
    Reminder,
    Achievement,
    FriendRequest
}
