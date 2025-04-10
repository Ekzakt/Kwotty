namespace Kwotty.Domain.Models;

public class Author
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string NameSlug { get; init; } = string.Empty;

    public Guid? MediumId { get; init; }

    public bool IsHidden { get; init; }

    public DateTime CreateOn { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public Medium? Medium { get; set; }
}
