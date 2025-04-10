namespace Kwotty.Domain.Models;

public class Quote
{
    public Guid Id { get; init; }

    public bool IsHidden { get; init; }

    public DateTime CreatedOn { get; init; }

    public string CreatedBy { get; init; } = string.Empty;
}  
