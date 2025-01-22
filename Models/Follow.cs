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
}

public enum FollowStatus
{
    Pending,
    Accepted
}
