namespace Ipitup.Models;
[Table("BadgeUser")]
public class BadgeUser
{
    [Key]
    [Column(Order = 0)]
    [JsonProperty("id")]
    public int BadgeId { get; set; }
    [Key]
    [Column(Order = 1)]
    [JsonProperty("userId")]
    public int UserId { get; set; }
    [ForeignKey("BadgeId")]
    public Badge? Badge { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
