namespace Kwotty.Application.Dtos.Quotes;

public record UpdateQuoteVisibilityDto(
    Guid Id,
    bool IsHidden
);