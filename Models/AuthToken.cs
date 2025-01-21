using System;

namespace Ipitup.Models
{
    public class AuthToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("tokenid")]
        public int TokenId { get; set; }
        [JsonProperty("userid")]
        public int UserId { get; set; }
        [JsonProperty("token")]
        public required string Token { get; set; }
        [JsonProperty("createdat")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("expiresat")]
        public DateTime ExpiresAt { get; set; }
        [JsonProperty("isvalid")]
        public bool IsValid { get; set; }
    }
}