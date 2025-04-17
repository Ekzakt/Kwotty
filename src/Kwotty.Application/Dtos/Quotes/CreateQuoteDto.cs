namespace Kwotty.Application.Dtos.Quotes;

public record CreateQuoteDto(
    Guid AuthorId,
    bool IsHidden,
    IEnumerable<CreateQuoteItemDto> quoteItems,
    IEnumerable<Guid> CategoryIds
);
