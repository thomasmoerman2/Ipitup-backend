using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("Exercise")]
public class Exercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public ExerciseType ExerciseType { get; set; }
}

public enum ExerciseType
{
    Armen,
    Benen,
    Rug,
    Schouders
}