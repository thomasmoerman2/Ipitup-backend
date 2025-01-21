namespace Ipitup.Models;

[Table("Follow")]
public class Follow
{
    [Key]
    [Column(Order = 0)]
    public int FollowerId { get; set; }

    [Key]
    [Column(Order = 1)]
    public int FollowingId { get; set; }

    public FollowStatus Status { get; set; } = FollowStatus.Pending;

    public DateTime FollowDate { get; set; } = DateTime.UtcNow;

    [ForeignKey("FollowerId")]
    public User? Follower { get; set; }

    [ForeignKey("FollowingId")]
    public User? Following { get; set; }
}

public enum FollowStatus
{
    Pending,
    Accepted
}
