using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


public class QuoteAccessToken
{
    [Key]
    public int Id { get; set; }


    [Required]
    [StringLength(128)]
    public string TokenValue { get; set; } = string.Empty;


    [Required]
    public Guid UserId { get; set; }


    [Required]
    public DateTimeOffset ExpiryUtc { get; set; }


    public DateTimeOffset? UsedUtc { get; set; }


    [Required]
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
