namespace Kwotty.Domain.Models;

/// <summary>
/// Represents the author of content (e.g., Quotes).
/// </summary>
public class Author
{
    private readonly List<Quote> _quotes = [];

    public Guid Id { get; }

    public string Name { get; private set; }

    public string? Slug { get; private set; }

    public bool IsHidden { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; }

    public string? CreatedBy { get; }

    public Medium? ProfileMedium { get; private set; }

    public IReadOnlyList<Quote> Quotes => _quotes.AsReadOnly();


    public Author(Guid id, string name, string? slug, bool isHidden, DateTimeOffset createdOnUtc, string? createdBy, Medium? profileMedium = null)
    {
        Id = id;
        Name = name; // Add validation
        Slug = slug; // Generate?
        IsHidden = isHidden;
        CreatedOnUtc = createdOnUtc;
        CreatedBy = createdBy;
        ProfileMedium = profileMedium;
    }


    public void UpdateDetails(string name, string? slug, bool isHidden, Medium? profileMedium)
    {
        Name = name;
        Slug = slug; // Regenerate?
        IsHidden = isHidden;
        ProfileMedium = profileMedium;
    }

    public void Hide() => IsHidden = true;

    public void Show() => IsHidden = false;



    #region Helpers

    internal void AddQuote(Quote quote)
    {
        if (!_quotes.Any(q => q.Id == quote.Id))
        { 
            _quotes.Add(quote);
        }
    }

    internal void RemoveQuote(Guid quoteId)
    {
        _quotes.RemoveAll(q => q.Id == quoteId);
    }

    #endregion Helpers
}