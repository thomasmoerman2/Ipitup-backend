namespace Ipitup_backend.Models;

[Table("Badge")]
public class Badge
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BadgeId { get; set; }
    [Required]
    [StringLength(100)]
    public string BadgeName { get; set; } = string.Empty;
    public string? BadgeDescription { get; set; }
    public int BadgeAmount { get; set; } = 0;
    public ICollection<BadgeUser> BadgeUsers { get; set; } = new List<BadgeUser>();
}
