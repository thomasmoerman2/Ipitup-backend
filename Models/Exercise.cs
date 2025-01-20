namespace Ipitup.Models;

[Table("Exercise")]
public class Exercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExerciseId { get; set; }

    [Required]
    [StringLength(100)]
    public string ExerciseName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ExerciseType { get; set; } = string.Empty;

    public string? ExerciseInstructions { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "ExerciseTime must be 0 or greater")]
    public int ExerciseTime { get; set; } = 30; // Default value zelfde als in database
}
