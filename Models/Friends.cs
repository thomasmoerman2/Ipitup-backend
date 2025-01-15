namespace Ipitup_backend.Models;
[Table("Friends")]
public class Friends
{
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public FriendStatus Status { get; set; } = FriendStatus.Waiting;
    [ForeignKey("UserId")]
    public User? User { get; set; }
    [ForeignKey("FriendId")]
    public User? FriendUser { get; set; }
}
public enum FriendStatus
{
    Waiting,
    Accepted
}
