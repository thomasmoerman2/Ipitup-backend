using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Exercises")]
public class Exercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExerciseId { get; set; }

    [Required]
    [StringLength(100)]
    public string ExerciseName { get; set; } = string.Empty;

    [Required]
    public ExerciseType ExerciseType { get; set; }
}

public enum ExerciseType
{
    Armen,
    Benen,
    Rug,
    Schouders
}