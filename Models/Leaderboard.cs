namespace Ipitup.Models;
[Table("Leaderboard")]
public class Leaderboard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int LeaderboardId { get; set; }
    [JsonProperty("userId")]
    public int UserId { get; set; }
    [JsonProperty("locationId")]
    public int? LocationId { get; set; }
    [JsonProperty("score")]
    public int Score { get; set; } = 0;
}
