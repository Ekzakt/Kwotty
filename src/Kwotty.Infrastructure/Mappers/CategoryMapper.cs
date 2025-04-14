using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mappers;

public class CategoryMapper : IMapper<Models.Category, Entities.Category>
{
    public Models.Category? ToModel(Entities.Category? entity)
    {
        if (entity == null) return null;
        return new Models.Category(
            entity.Id,
            entity.Name,
            entity.NameSlug,
            entity.IsHidden,
            entity.CreatedOnUtc, // Persistence property name
            entity.CreatedBy
        );
    }

    public IEnumerable<Models.Category> ToModelList(IEnumerable<Entities.Category>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.Category>();
    }

    public Entities.Category? ToNewEntity(Models.Category? domain)
    {
        if (domain == null) return null;
        return new Entities.Category
        {
            Id = domain.Id,
            Name = domain.Name,
            NameSlug = domain.NameSlug,
            IsHidden = domain.IsHidden,
            CreatedOnUtc = domain.CreateOnUtc, // Domain property name (has typo?)
            CreatedBy = domain.CreatedBy
        };
    }

    public void ApplyUpdate(Models.Category domain, Entities.Category entity)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        entity.Name = domain.Name;
        entity.NameSlug = domain.NameSlug;
        entity.IsHidden = domain.IsHidden;
        // Id, CreatedOnUtc, CreatedBy not updated.
        // UserCategories/QuoteCategories relationships updated via UserSettings/Quote mappers/repos.
    }
}