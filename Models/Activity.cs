namespace Ipitup_backend.Models;
[Table("Activity")]
public class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ActivityId { get; set; }
    public int UserId { get; set; }
    public int ActivityAmount { get; set; }
    public int ActivityDuration { get; set; }
    public DateTime ActivityDate { get; set; } = DateTime.Now;
    public int? LocationId { get; set; }
    public int? ExerciseId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
    [ForeignKey("LocationId")]
    public Location? Location { get; set; }
    [ForeignKey("ExerciseId")]
    public Exercise? Exercise { get; set; }
}
