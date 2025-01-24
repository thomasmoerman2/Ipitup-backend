namespace Ipitup.Models;
[Table("Activity")]
public class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int ActivityId { get; set; }
    [Required]
    [JsonProperty("userId")]
    public int UserId { get; set; }
    [Required]
    [JsonProperty("activityScore")]
    public int ActivityScore { get; set; }
    [Required]
    [JsonProperty("activityDuration")]
    public int ActivityDuration { get; set; } // in seconden
    [JsonProperty("activityDate")]
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    [JsonProperty("locationId")]
    public int? LocationId { get; set; }
    [JsonProperty("exerciseId")]
    public int? ExerciseId { get; set; }
}
