using Kwotty.Application.Contracts;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Mappers;

public class UserSettingsMapper : IMapper<Models.UserSettings, Entities.UserSettings>
{
    private readonly IMapper<Models.Category, Entities.Category> _categoryMapper;

    public UserSettingsMapper(IMapper<Models.Category, Entities.Category> categoryMapper)
    {
        _categoryMapper = categoryMapper ?? throw new ArgumentNullException(nameof(categoryMapper));
    }
    public UserSettingsMapper() : this(new CategoryMapper()) { }


    public Models.UserSettings? ToModel(Entities.UserSettings? entity)
    {
        if (entity == null) return null;
        var userSettingsModel = new Models.UserSettings(entity.UserId);

        // Apply state
        if (entity.IsPaused) userSettingsModel.Pause(); else userSettingsModel.Resume(); // Use domain methods
        // Reflect exact timestamps from persistence if they differ from UtcNow in methods
        typeof(Models.UserSettings).GetProperty("LastPausedOnUtc")?.SetValue(userSettingsModel, entity.LastPausedOn); // Map to correct domain property
        typeof(Models.UserSettings).GetProperty("LastResumedOnUtc")?.SetValue(userSettingsModel, entity.LastResumedOn); // Map to correct domain property

        // Map categories
        if (entity.UserCategories != null)
        {
            var categories = entity.UserCategories
                .Where(uc => uc.Category != null)
                .Select(uc => _categoryMapper.ToModel(uc.Category!)) // Use injected mapper
                .Where(c => c != null).ToList();

            if (categories.Any()) userSettingsModel.SetInitialCategories(categories!); // Use internal setter
        }

        return userSettingsModel;
    }

    public IEnumerable<Models.UserSettings> ToModelList(IEnumerable<Entities.UserSettings>? entities)
    {
        return entities?.Select(ToModel).Where(d => d != null).ToList() ?? Enumerable.Empty<Models.UserSettings>();
    }

    public Entities.UserSettings? ToNewEntity(Models.UserSettings? domain)
    {
        if (domain == null) return null;
        var entity = new Entities.UserSettings
        {
            UserId = domain.UserId, // Must match User.Id
            IsPaused = domain.IsPaused, // Map domain name to entity name
            LastPausedOn = domain.LastPausedOnUtc, // Map domain name to entity name
            LastResumedOn = domain.LastResumedOnUtc, // Map domain name to entity name
            // Map preferred categories -> UserCategory join entities (Complex: Requires Service/Repo)
            UserCategories = domain.PreferredCategories?
                .Select(catDomain => new Entities.UserCategory { UserId = domain.UserId, CategoryId = catDomain.Id })
                .ToHashSet() ?? new HashSet<Entities.UserCategory>()
        };
        return entity;
    }

    public void ApplyUpdate(Models.UserSettings domain, Entities.UserSettings entity)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(entity);

        entity.IsPaused = domain.IsPaused; // Map domain name to entity name
        entity.LastPausedOn = domain.LastPausedOnUtc; // Map domain name to entity name
        entity.LastResumedOn = domain.LastResumedOnUtc; // Map domain name to entity name
        // UserId not updated.
        // UserCategories collection updates need complex handling (Service/Repo).
    }
}