namespace Kwotty.Domain.Models;

public class QuoteItem
{
    public Guid Id { get; init; }

    public Guid QuoteId { get; init; }

    public string Text { get; init; } = string.Empty;

    public string TextSlug { get; init; } = string.Empty;

    public Guid AuthorId { get; init; }

    public Guid MeaiumId { get; init; }

    public Author? Author { get; init; }

}
