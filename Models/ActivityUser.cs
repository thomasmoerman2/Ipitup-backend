namespace Ipitup_backend.Models;
[Table("ActivityUser")]
public class ActivityUser
{
    public int ActivityId { get; set; }
    public int UserId { get; set; }
    [ForeignKey("ActivityId")]
    public Activity? Activity { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
