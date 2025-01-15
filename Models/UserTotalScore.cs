namespace Ipitup_backend.Models;
[Table("UserTotalScore")]
public class UserTotalScore
{
    [Key]
    public int UserId { get; set; }
    public int TotalScore { get; set; } = 0;
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
