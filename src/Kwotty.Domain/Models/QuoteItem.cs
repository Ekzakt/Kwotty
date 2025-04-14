namespace Kwotty.Domain.Models;

/// <summary>
/// Represents an individual item/line within a Quote.
/// Owned by the Quote aggregate.
/// </summary>
public class QuoteItem
{
    public Guid Id { get; }
    public string? Text { get; private set; }
    public string? TextSlug { get; private set; }
    public int SortNumber { get; private set; }


    public Medium? Medium { get; private set; }


    public QuoteItem(Guid id, string? text, string? textSlug, int sortNumber, Medium? medium = null)
    {
        Id = id;
        Text = text;
        TextSlug = textSlug; // Generate?
        SortNumber = sortNumber;
        Medium = medium;
    }


    public void Update(string? text, string? textSlug, int sortNumber, Medium? medium)
    {
        Text = text;
        TextSlug = textSlug; // Regenerate?
        SortNumber = sortNumber;
        Medium = medium;
    }
}
