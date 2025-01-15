namespace Ipitup_backend.Models;
[Table("BadgeUser")]
public class BadgeUser
{
    public int BadgeId { get; set; }
    public int UserId { get; set; }
    [ForeignKey("BadgeId")]
    public Badge? Badge { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
