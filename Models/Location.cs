namespace Ipitup.Models;
[Table("Location")]
public class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int LocationId { get; set; }
    [Required]
    [StringLength(100)]
    [JsonProperty("locationName")]
    public string LocationName { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    [JsonProperty("locationCountry")]
    public string LocationCountry { get; set; } = string.Empty;
}
