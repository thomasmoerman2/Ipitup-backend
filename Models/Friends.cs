namespace Ipitup_backend.Models;
[Table("Friends")]
public class Friends
{
    [Key]
    [Column(Order = 0)]
    public int UserId { get; set; }
    [Key]
    [Column(Order = 1)]
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
