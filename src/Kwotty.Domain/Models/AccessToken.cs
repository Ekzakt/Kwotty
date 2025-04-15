namespace Kwotty.Domain.Models;

public class AccessToken
{
    public int Id { get; set; }

    public string TokenValue { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTimeOffset ExpiresOnUtc { get; set; }

    public DateTimeOffset? UsedOnUtc { get; set; }

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
