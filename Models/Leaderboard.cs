namespace Ipitup_backend.Models;
[Table("Leaderboard")]
public class Leaderboard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LeaderboardId { get; set; }
    public int UserId { get; set; }
    public int? LocationId { get; set; }
    public int Score { get; set; } = 0;
    [ForeignKey("UserId")]
    public User? User { get; set; }
    [ForeignKey("LocationId")]
    public Location? Location { get; set; }
}
