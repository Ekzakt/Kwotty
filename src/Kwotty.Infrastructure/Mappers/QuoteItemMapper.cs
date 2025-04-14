using Kwotty.Application.Contracts;
using Entities = Kwotty.Data.Entities;
using Models = Kwotty.Domain.Models;

namespace Kwotty.Infrastructure.Mappers;

public class QuoteItemMapper : IMapper<Models.QuoteItem, Entities.QuoteItem>
{
    private readonly IMapper<Models.Medium, Entities.Medium> _mediumMapper;

    public QuoteItemMapper(IMapper<Models.Medium, Entities.Medium> mediumMapper)
    {
        _mediumMapper = mediumMapper ?? throw new ArgumentNullException(nameof(mediumMapper));
    }

    public Models.QuoteItem? ToModel(Entities.QuoteItem? entity)
    {
        if (entity == null) return null;

        var mediumModel = _mediumMapper.ToModel(entity.Medium);

        return new Models.QuoteItem(
            entity.Id,
            entity.Text,
            entity.TextSlug,
            entity.SortNumber,
            mediumModel // Pass mapped medium
        );
    }

    public IEnumerable<Models.QuoteItem> ToModelList(IEnumerable<Entities.QuoteItem>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.QuoteItem>();
    }

    public Entities.QuoteItem? ToNewEntity(Models.QuoteItem? domain)
    {
        if (domain == null) return null;

        return new Entities.QuoteItem
        {
            Id = domain.Id,
            Text = domain.Text,
            TextSlug = domain.TextSlug,
            SortNumber = domain.SortNumber,
            MediumId = domain.Medium?.Id, // Use correct domain property & set FK
            // QuoteId is set by EF when adding to a tracked Quote entity's collection
        };
    }

    public void ApplyUpdate(Models.QuoteItem domain, Entities.QuoteItem entity)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        entity.Text = domain.Text;
        entity.TextSlug = domain.TextSlug;
        entity.SortNumber = domain.SortNumber;
        entity.MediumId = domain.Medium?.Id; // Update FK
        // Id and QuoteId typically not updated here.
    }
}
