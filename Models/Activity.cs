namespace Ipitup.Models;

[Table("Activity")]
public class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ActivityId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ActivityScore { get; set; }

    [Required]
    public int ActivityDuration { get; set; } // in seconden

    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;

    public int? LocationId { get; set; }
    public int? ExerciseId { get; set; }
}
