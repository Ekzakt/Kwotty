using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mapper;

public class MediumMapper : IMapper<Models.Medium, Entities.Medium>
{
    public Models.Medium? ToModel(Entities.Medium? entity)
    {
        if (entity == null) return null;
        return new Models.Medium(
            entity.Id,
            entity.BaseUrl,
            entity.FileName,
            entity.CompleteUrl,
            entity.IsHidden,
            entity.CreatedOnUtc,
            entity.CreatedBy
        );
    }

    public IEnumerable<Models.Medium> ToModelList(IEnumerable<Entities.Medium>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.Medium>();
    }

    public Entities.Medium? ToNewEntity(Models.Medium? domain)
    {
        if (domain == null) return null;
        return new Entities.Medium
        {
            Id = domain.Id,
            BaseUrl = domain.BaseUrl,
            FileName = domain.FileName,
            CompleteUrl = domain.CompleteUrl,
            IsHidden = domain.IsHidden,
            CreatedOnUtc = domain.CreatedOnUtc,
            CreatedBy = domain.CreatedBy
        };
    }

    public void ApplyUpdate(Models.Medium domain, Entities.Medium entity)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        entity.BaseUrl = domain.BaseUrl;
        entity.FileName = domain.FileName;
        entity.CompleteUrl = domain.CompleteUrl;
        entity.IsHidden = domain.IsHidden;
        // Id, CreatedOnUtc, CreatedBy not updated.
    }
}