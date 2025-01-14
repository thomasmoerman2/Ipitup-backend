using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("BadgeUsers")]
public class BadgeUser
{
    [Key]
    [Column(Order = 1)]
    public int BadgeId { get; set; }

    [Key]
    [Column(Order = 2)]
    public int UserId { get; set; }

    // Navigation properties
    [ForeignKey("BadgeId")]
    public Badge? Badge { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}