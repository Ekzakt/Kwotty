namespace Kwotty.Domain.Models;

public class Medium
{
    public Guid Id { get; init; }

    public string BaseUrl { get; init; } = string.Empty;

    public string FileName { get; init; } = string.Empty;

    public string CompleteUrl { get; init; } = string.Empty;

    public bool IsHidden { get; init; }

    public DateTime CreatedOn { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public List< >
}
