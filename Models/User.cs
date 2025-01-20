namespace Ipitup.Models;
[Table("User")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    [Required]
    [StringLength(255)]
    public string Avatar { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string UserFirstname { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string UserLastname { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    [JsonProperty("username")]
    public string UserEmail { get; set; } = string.Empty;
    [Required]
    [StringLength(255)]
    [JsonProperty("password")]
    public string UserPassword { get; set; } = string.Empty;
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Public;
    public int DailyStreak { get; set; } = 0;
    public DateTime BirthDate { get; set; }
    public int TotalScore { get; set; } = 0;
}
public enum AccountStatus
{
    Public,
    Private
}
