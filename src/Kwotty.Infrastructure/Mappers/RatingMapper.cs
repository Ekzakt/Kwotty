using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mappers;

public class RatingMapper : IMapper<Models.Rating, Entities.Rating>
{
    public Models.Rating? ToModel(Entities.Rating? entity)
    {
        if (entity == null) return null;

        // Use reflection to call internal constructor or add a public factory method to Models.Rating
        var constructor = typeof(Models.Rating).GetConstructor(
             System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public, null,
             new Type[] { typeof(Guid), typeof(Guid), typeof(byte), typeof(DateTimeOffset) }, null);

        if (constructor != null)
        {
            return (Models.Rating)constructor.Invoke(new object[] { entity.QuoteId, entity.UserId, entity.Value, entity.RatedOnUtc });
        }
        // Handle constructor access error (e.g., log, throw, or return null)
        Console.Error.WriteLine($"Error: Could not access internal constructor for Models.Rating when mapping Entities.Rating Id={entity.Id}.");
        return null; // Or throw
    }

    public IEnumerable<Models.Rating> ToModelList(IEnumerable<Entities.Rating>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.Rating>();
    }

    public Entities.Rating? ToNewEntity(Models.Rating? domain)
    {
        if (domain == null) return null;
        return new Entities.Rating
        {
            // Id (long) is DB generated
            QuoteId = domain.QuoteId, // Assumes Quote exists/is tracked
            UserId = domain.UserId,   // Assumes User exists/is tracked
            Value = domain.Value,
            RatedOnUtc = domain.RatedOn // Map domain name to entity name
        };
    }

    public void ApplyUpdate(Models.Rating domain, Entities.Rating entity)
    {
        // This assumes you identify the correct 'entity' using QuoteId/UserId
        // in the repository/service before calling this.
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        // Ensure we are updating the correct rating entity if found via composite key
        if (entity.QuoteId == domain.QuoteId && entity.UserId == domain.UserId)
        {
            entity.Value = domain.Value;
            entity.RatedOnUtc = domain.RatedOn;
        }
        else
        {
            // Log or throw if the wrong entity was passed for update
            throw new ArgumentException($"ApplyUpdate called on Entities.Rating Id={entity.Id} which does not match Models.Rating composite key ({domain.QuoteId}/{domain.UserId}).");
        }
        // Id, QuoteId, UserId are not updated.
    }
}