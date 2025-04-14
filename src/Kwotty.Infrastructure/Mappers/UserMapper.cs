using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mappers;

public class UserMapper : IMapper<Models.User, Entities.User>
{
    // Dependencies
    private readonly IMapper<Models.UserSettings, Entities.UserSettings> _settingsMapper;
    private readonly IMapper<Models.Rating, Entities.Rating> _ratingMapper;

    public UserMapper(
        IMapper<Models.UserSettings, Entities.UserSettings> settingsMapper,
        IMapper<Models.Rating, Entities.Rating> ratingMapper)
    {
        _settingsMapper = settingsMapper ?? throw new ArgumentNullException(nameof(settingsMapper));
        _ratingMapper = ratingMapper ?? throw new ArgumentNullException(nameof(ratingMapper));
    }

    public Models.User? ToModel(Entities.User? entity)
    {
        if (entity == null) return null;

        var userSettingsModel = _settingsMapper.ToModel(entity.Settings);

        var userModel = new Models.User(
            entity.Id,
            entity.Email,
            entity.Firstname,
            userSettingsModel,
            entity.IsDisabled
        );

        // Map Ratings given by user
        if (entity.QuoteRatings != null)
        {
            // Models.User needs a method to accept the initial list if managing it.
            // The provided Models.User doesn't have AddRatingReference/SetInitialGivenRatings.
            // Assuming for now the list isn't populated on the User domain object directly from here.
            // If needed, add SetInitialGivenRatings(IEnumerable<Rating>) to Models.User and call it:
            // var ratings = _ratingMapper.ToDomainList(entity.QuoteRatings).ToList();
            // if (ratings.Any()) domainUser.SetInitialGivenRatings(ratings);
        }

        return userModel;
    }

    public IEnumerable<Models.User> ToModelList(IEnumerable<Entities.User>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.User>();
    }

    public Entities.User? ToNewEntity(Models.User? domain)
    {
        if (domain == null) return null;
        var entity = new Entities.User
        {
            Id = domain.Id,
            Email = domain.Email,
            Firstname = domain.Firstname,
            IsDisabled = domain.IsDisabled,
            CreateOnUtc = DateTimeOffset.UtcNow, // Set on creation; Domain model lacks this
            Settings = _settingsMapper.ToNewEntity(domain.Settings),
            // Map Ratings (Requires handling existing Quotes in Service/Repo)
            QuoteRatings = domain.GivenRatings? // Use domain property name
                .Select(_ratingMapper.ToNewEntity) // Use injected mapper
                .Where(r => r != null)
                .ToHashSet()! ?? new HashSet<Entities.Rating>()
        };
        // Ensure FKs are set
        if (entity.Settings != null) entity.Settings.UserId = entity.Id;
        foreach (var ratingEntity in entity.QuoteRatings) { ratingEntity.UserId = entity.Id; }

        return entity;
    }

    public void ApplyUpdate(Models.User domain, Entities.User entity)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        entity.Email = domain.Email;
        entity.Firstname = domain.Firstname;
        entity.IsDisabled = domain.IsDisabled;
        // Id, CreateOnUtc not updated.
        // UserSettings relationship update needs careful handling (usually via UserSettings ApplyUpdate).
        // QuoteRatings collection updates need complex handling (Service/Repo).
    }
}