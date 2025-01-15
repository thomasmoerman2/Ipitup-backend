namespace Ipitup_backend.Models;
[Table("Badge")]
public class Badge
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BadgeId { get; set; }
    public string BadgeName { get; set; } = string.Empty;
    public string? BadgeDescription { get; set; }
    public ICollection<BadgeUser> BadgeUsers { get; set; } = new List<BadgeUser>();
}
