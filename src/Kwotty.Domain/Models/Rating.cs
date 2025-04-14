namespace Kwotty.Domain.Models;

/// <summary>
/// Value Object representing a rating given BY a User.
/// </summary>
public class Rating
{
    public Guid QuoteId { get; }

    public Guid UserId { get; }

    public byte Value { get; private set; }

    public DateTimeOffset RatedOn { get; private set; }



    #region Helpers

    internal Rating(Guid quoteId, Guid userId, byte value, DateTimeOffset ratedOn)
    {
        QuoteId = quoteId;
        UserId = userId;
        Value = value;
        RatedOn = ratedOn;
    }

    internal void UpdateRating(byte value, DateTimeOffset ratedOn)
    {
        Value = value;
        RatedOn = ratedOn;
    }

    #endregion Helpers
}
