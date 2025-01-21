namespace Ipitup.Models;
[Table("Leaderboard")]
public class Leaderboard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LeaderboardId { get; set; }
    public int UserId { get; set; }
    public int? LocationId { get; set; }
    public int Score { get; set; } = 0;
}
