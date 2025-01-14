using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Activities")]
public class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ActivityId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ActivityAmount { get; set; }

    [Required]
    public int ActivityDuration { get; set; }

    [Required]
    public DateTime ActivityDate { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public int ExerciseId { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("LocationId")]
    public Location? Location { get; set; }

    [ForeignKey("ExerciseId")]
    public Exercise? Exercise { get; set; }
}