namespace Ipitup.Models;
[Table("User")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonProperty("id")]
    public int UserId { get; set; }
    [Required]
    [StringLength(255)]
    [JsonProperty("avatar")]
    public string Avatar { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    [JsonProperty("firstname")]
    public string UserFirstname { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    [JsonProperty("lastname")]
    public string UserLastname { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    [JsonProperty("email")]
    public string UserEmail { get; set; } = string.Empty;
    [Required]
    [StringLength(255)]
    [JsonProperty("password")]
    public string UserPassword { get; set; } = string.Empty;
    [JsonProperty("accountstatus")]
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Public;
    [JsonProperty("dailystreak")]
    public int DailyStreak { get; set; } = 0;
    [JsonProperty("birthdate")]
    public DateTime BirthDate { get; set; }
    [JsonProperty("totalscore")]
    public int TotalScore { get; set; } = 0;
    [JsonProperty("isAdmin")]
    public bool IsAdmin { get; set; } = false;
}
public enum AccountStatus
{
    Public,
    Private
}
