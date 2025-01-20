namespace Ipitup_backend.Models;
[Table("Location")]
public class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LocationId { get; set; }
    [Required]
    [StringLength(100)]
    public string LocationName { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string LocationCountry { get; set; } = string.Empty;
}
