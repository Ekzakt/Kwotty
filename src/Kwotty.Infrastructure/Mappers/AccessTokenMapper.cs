using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mappers;

public class AccessTokenMapper : IMapper<Models.AccessToken, Entities.AccessToken>
{
    public Models.AccessToken? ToModel(Entities.AccessToken? entity)
    {
        if (entity == null) return null;

        return new Models.AccessToken
        {
            Id = entity.Id,
            TokenValue = entity.TokenValue,
            UserId = entity.UserId,
            ExpiresOnUtc = entity.ExpiresOnUtc,
            // Mapping Persistence.ExpiresOn (nullable DateTimeOffset) to Domain.UsedOnUtc (nullable DateTimeOffset)
            // Verify if this mapping is correct based on your application logic.
            UsedOnUtc = entity.ExpiresOn,
            CreatedUtc = entity.CreatedOnUtc // Map Persistence.CreatedOnUtc -> Domain.CreatedUtc
        };
    }

    public IEnumerable<Models.AccessToken> ToModelList(IEnumerable<Entities.AccessToken>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.AccessToken>();
    }

    public Entities.AccessToken? ToNewEntity(Models.AccessToken? model)
    {
        if (model == null) return null;

        return new Entities.AccessToken
        {
            // Id is typically DB generated for int keys
            TokenValue = model.TokenValue,
            UserId = model.UserId,
            ExpiresOnUtc = model.ExpiresOnUtc,
            ExpiresOn = model.UsedOnUtc, // Mapping Domain.UsedOnUtc -> Persistence.ExpiresOn? Verify.
            CreatedOnUtc = model.CreatedUtc // Map Domain.CreatedUtc -> Persistence.CreatedOnUtc
        };
    }

    public void ApplyUpdate(Models.AccessToken domain, Entities.AccessToken entity)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        entity.TokenValue = domain.TokenValue;
        entity.UserId = domain.UserId; // Should UserId change? Usually not after creation.
        entity.ExpiresOnUtc = domain.ExpiresOnUtc;
        entity.ExpiresOn = domain.UsedOnUtc; // Verify mapping
        // Id and CreatedOnUtc typically not updated.
    }
}