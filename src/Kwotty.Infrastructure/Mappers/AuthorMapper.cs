using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mappers;

public class AuthorMapper : IMapper<Models.Author, Entities.Author>
{
    private readonly IMapper<Models.Medium, Entities.Medium> _mediumMapper;

    public AuthorMapper(IMapper<Models.Medium, Entities.Medium> mediumMapper)
    {
        _mediumMapper = mediumMapper ?? throw new ArgumentNullException(nameof(mediumMapper));
    }

    public Models.Author? ToModel(Entities.Author? entity)
    {
        if (entity == null) return null;

        var mediumModel = _mediumMapper.ToModel(entity.Medium);

        var domainAuthor = new Models.Author(
            entity.Id,
            entity.Name,
            entity.Slug,
            entity.IsHidden,
            entity.CreatedOnUtc,
            entity.CreatedBy,
            mediumModel
        );

        // Note: Domain.Author.Quotes collection is not populated here.
        // It should be loaded explicitly when needed (e.g., GetAuthorWithQuotes).

        return domainAuthor;
    }

    public IEnumerable<Models.Author> ToModelList(IEnumerable<Entities.Author>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.Author>();
    }

    public Entities.Author? ToNewEntity(Models.Author? model)
    {
        if (model == null) return null;
        return new Entities.Author
        {
            Id = model.Id, // Assumes ID is set before calling
            Name = model.Name,
            Slug = model.Slug,
            IsHidden = model.IsHidden,
            CreatedOnUtc = model.CreatedOnUtc,
            CreatedBy = model.CreatedBy,
            MediumId = model.ProfileMedium?.Id // Set FK
            // Medium navigation property not set here; EF handles via FK.
            // Quotes collection not set here; managed by Quote entity's AuthorId.
        };
    }

    public void ApplyUpdate(Models.Author model, Entities.Author entity)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);

        entity.Name = model.Name;
        entity.Slug = model.Slug;
        entity.IsHidden = model.IsHidden;
        entity.MediumId = model.ProfileMedium?.Id; // Update FK

        // Id, CreatedOnUtc, CreatedBy are not updated.
        // Quotes collection changes are managed via Quote entity.
    }
}