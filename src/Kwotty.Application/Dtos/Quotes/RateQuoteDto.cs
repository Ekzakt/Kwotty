namespace Kwotty.Application.Dtos.Quotes;

public record RateQuoteDto(
     Guid UserId,
     byte RatingValue
);