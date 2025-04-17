namespace Kwotty.Application.Dtos.Quotes;

public record CreateQuoteItemDto(
    string? Text,
    int SortOrder,
    Guid? MediumId
);
