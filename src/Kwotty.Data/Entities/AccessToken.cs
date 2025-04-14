using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


public class AccessToken
{
    [Key]
    public int Id { get; set; }


    [Required]
    [StringLength(128)]
    public string TokenValue { get; set; } = string.Empty;


    [Required]
    public Guid UserId { get; set; }


    [Required]
    public DateTimeOffset ExpiresOnUtc { get; set; }


    public DateTimeOffset? ExpiresOn { get; set; }


    [Required]
    public DateTimeOffset CreateOnUtc { get; set; } = DateTimeOffset.UtcNow;
}
