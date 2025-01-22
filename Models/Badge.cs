namespace Ipitup.Models;
[Table("Badge")]
public class Badge
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int BadgeId { get; set; }
    [Required]
    [StringLength(100)]
    [JsonProperty("badgeName")]
    public string BadgeName { get; set; } = string.Empty;
    [JsonProperty("badgeDescription")]
    public string? BadgeDescription { get; set; }
    [JsonProperty("badgeAmount")]
    public int BadgeAmount { get; set; } = 0;
    [JsonProperty("badgeUsers")]
    public ICollection<BadgeUser> BadgeUsers { get; set; } = new List<BadgeUser>();
}
