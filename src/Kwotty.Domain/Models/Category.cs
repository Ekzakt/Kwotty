namespace Kwotty.Domain.Models;

/// <summary>
/// Represents a category for organizing content (e.g., Quotes).
/// </summary>
public class Category
{
    public Guid Id { get; }

    public string Name { get; private set; }

    public string? NameSlug { get; private set; }

    public bool IsHidden { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; }

    public string? CreatedBy { get; }


    public Category(Guid id, string name, string? nameSlug, bool isHidden, DateTimeOffset createdAt, string? createdBy)
    {
        Id = id;
        Name = name; // Add validation if required
        NameSlug = nameSlug; // Could be generated
        IsHidden = isHidden;
        CreatedOnUtc = createdAt;
        CreatedBy = createdBy;
    }


    public void UpdateDetails(string name, string? nameSlug, bool isHidden)
    {
        // Add validation
        Name = name;
        NameSlug = nameSlug; // Regenerate if needed?
        IsHidden = isHidden;
    }

    public void Hide() => IsHidden = true;

    public void Show() => IsHidden = false;
}
