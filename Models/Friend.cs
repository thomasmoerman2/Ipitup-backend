using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Friends")]
public class Friend
{
    [Key]
    [Column(Order = 1)]
    public int UserId { get; set; }

    [Key]
    [Column(Order = 2)]
    public int FriendId { get; set; }

    [Required]
    public FriendStatus Status { get; set; }

    // Navigation properties
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