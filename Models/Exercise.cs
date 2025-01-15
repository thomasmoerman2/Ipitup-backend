namespace Ipitup_backend.Models;
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
