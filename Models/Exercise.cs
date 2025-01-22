namespace Ipitup.Models;

[Table("Exercise")]
public class Exercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int ExerciseId { get; set; }

    [Required]
    [StringLength(100)]
    [JsonProperty("exerciseName")]
    public string ExerciseName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [JsonProperty("exerciseType")]
    public string ExerciseType { get; set; } = string.Empty;

    [JsonProperty("exerciseInstructions")]
    public string? ExerciseInstructions { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "ExerciseTime must be 0 or greater")]
    [JsonProperty("exerciseTime")]
    public int ExerciseTime { get; set; } = 30; // Default value zelfde als in database
}
