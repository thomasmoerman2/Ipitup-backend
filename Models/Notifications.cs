namespace Ipitup.Models;
[Table("Notifications")]
public class Notifications
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int NotificationId { get; set; }
    [JsonProperty("userId")]
    public int UserId { get; set; }
    [Required]
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
    [JsonProperty("isRead")]
    public bool IsRead { get; set; } = false;
    [JsonProperty("type")]
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
