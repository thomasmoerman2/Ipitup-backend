using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Badges")]
public class Badge
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BadgeId { get; set; }

    [Required]
    [StringLength(100)]
    public string BadgeName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "text")]
    public string BadgeDescription { get; set; } = string.Empty;

    // Navigation property for the many-to-many relationship with users
    public ICollection<BadgeUser> BadgeUsers { get; set; } = new List<BadgeUser>();
}