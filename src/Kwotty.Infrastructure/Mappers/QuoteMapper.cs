using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mappers;

public class QuoteMapper : IMapper<Models.Quote, Entities.Quote>
{
    // Dependencies on other mappers
    private readonly IMapper<Models.Author, Entities.Author> _authorMapper;
    private readonly IMapper<Models.QuoteItem, Entities.QuoteItem> _itemMapper;
    private readonly IMapper<Models.Category, Entities.Category> _categoryMapper;
    private readonly IMapper<Models.Rating, Entities.Rating> _ratingMapper;

    // Constructor injection for dependencies
    public QuoteMapper(
        IMapper<Models.Author, Entities.Author> authorMapper,
        IMapper<Models.QuoteItem, Entities.QuoteItem> itemMapper,
        IMapper<Models.Category, Entities.Category> categoryMapper,
        IMapper<Models.Rating, Entities.Rating> ratingMapper)
    {
        _authorMapper = authorMapper ?? throw new ArgumentNullException(nameof(authorMapper));
        _itemMapper = itemMapper ?? throw new ArgumentNullException(nameof(itemMapper));
        _categoryMapper = categoryMapper ?? throw new ArgumentNullException(nameof(categoryMapper));
        _ratingMapper = ratingMapper ?? throw new ArgumentNullException(nameof(ratingMapper));
    }

    public Models.Quote? ToModel(Entities.Quote? entity)
    {
        if (entity == null) return null;

        // Map Author (required dependency)
        var authorModel = _authorMapper.ToModel(entity.Author);

        if (authorModel == null && entity.AuthorId != Guid.Empty)
        {
            // Log warning or error: Author data missing for Quote
            Console.Error.WriteLine($"Warning: Author {entity.AuthorId} not loaded or found for Quote {entity.Id}.");
            return null; // Cannot create Quote model without Author
        }

        if (authorModel == null) return null;


        // Map Items (owned collection)
        var itemsModel = _itemMapper.ToModelList(entity.QuoteItems)
                                     .OrderBy(i => i.SortNumber)
                                     .ToList();

        var categoriesModel = entity.QuoteCategories
            .Select(qc => _categoryMapper.ToModel(qc.Category)) // Map the Category part
            .Where(c => c != null)
            .ToList();

        // Create Model object
        var quoteModel = new Models.Quote(
            entity.Id,
            authorModel,
            entity.IsHidden,
            entity.CreatedOnUtc,
            entity.CreatedBy,
            itemsModel,
            categoriesModel
        );

        // Map Categories (from join table)
        //if (entity.QuoteCategories != null)
        //{
        //    var categoriesModel = entity.QuoteCategories
        //        .Select(qc => _categoryMapper.ToModel(qc.Category)) // Map the Category part
        //        .Where(c => c != null)
        //        .ToList();

        //    if (categoriesModel.Any()) quoteModel.SetInitialCategories(categoriesModel!); // Use internal setter
        //}

        // Map Ratings (from join table)
        if (entity.QuoteRatings != null)
        {
            var ratingModel = _ratingMapper.ToModelList(entity.QuoteRatings).ToList();
            if (ratingModel.Any()) quoteModel.SetInitialRatings(ratingModel); // Use internal setter
        }

        return quoteModel;
    }

    public IEnumerable<Models.Quote> ToModelList(IEnumerable<Entities.Quote>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.Quote>();
    }

    public Entities.Quote? ToNewEntity(Models.Quote? model)
    {
        if (model == null) return null;

        var entity = new Entities.Quote
        {
            Id = model.Id,
            IsHidden = model.IsHidden,
            CreatedOnUtc = model.CreatedOnUtc,
            CreatedBy = model.CreatedBy,
            AuthorId = model.Author.Id, // Set Author FK

            // Map owned QuoteItems
            QuoteItems = model.Items?
                .Select(_itemMapper.ToNewEntity) // Use injected mapper
                .Where(qi => qi != null)
                .ToHashSet()! ?? new HashSet<Entities.QuoteItem>(),

            // Map Categories -> creates join entities (QuoteCategory)
            // Assumes Categories exist. Attaching existing needs DbContext.
            QuoteCategories = model.Categories?
                .Select(catDomain => new Entities.QuoteCategory { QuoteId = model.Id, CategoryId = catDomain.Id })
                .ToHashSet() ?? new HashSet<Entities.QuoteCategory>(),

            // Map Ratings -> creates join entities (Rating)
            // Assumes Users exist. Attaching existing needs DbContext.
            QuoteRatings = model.Ratings? // Use correct domain property
                .Select(_ratingMapper.ToNewEntity) // Use injected mapper
                .Where(r => r != null)
                .ToHashSet()! ?? new HashSet<Entities.Rating>()
        };

        // Ensure FKs are set on newly created Rating entities
        foreach (var ratingEntity in entity.QuoteRatings)
        { 
            ratingEntity.QuoteId = entity.Id;
        }
        // EF handles QuoteId on QuoteItems when added to collection

        return entity;
    }

    public void ApplyUpdate(Models.Quote domain, Entities.Quote entity)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        entity.IsHidden = domain.IsHidden;
        entity.AuthorId = domain.Author.Id; // Update FK if Author can change

        // WARNING: Collection synchronization (Items, Categories, Ratings) is NOT handled here.
        // This requires complex logic comparing collections and interacting with the DbContext
        // within the Repository or Service layer.
        // Example: You would need to load entity.QuoteItems, entity.QuoteCategories, entity.QuoteRatings,
        // compare them with domain.Items, domain.Categories, domain.ReceivedRatings,
        // identify items/links to add, remove, or update, and modify the DbContext state accordingly.
    }
}