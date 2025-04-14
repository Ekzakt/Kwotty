namespace Kwotty.Application.Contracts;

/// <summary>
/// Defines a contract for mapping between Domain models and Persistence entities.
/// </summary>
/// <typeparam name="TModel">The Domain model type.</typeparam>
/// <typeparam name="TEntity">The Persistence entity type.</typeparam>
public interface IMapper<TModel, TEntity>
    where TModel : class
    where TEntity : class
{
    /// <summary>
    /// Maps a Persistence entity to a Domain model.
    /// </summary>
    /// <param name="entity">The persistence entity.</param>
    /// <returns>The corresponding domain model, or null if the entity was null.</returns>
    TModel? ToModel(TEntity? entity);

    /// <summary>
    /// Maps a collection of Persistence entities to a collection of Domain models.
    /// </summary>
    /// <param name="entities">The collection of persistence entities.</param>
    /// <returns>An IEnumerable of corresponding domain models.</returns>
    IEnumerable<TModel> ToModelList(IEnumerable<TEntity>? entities);

    /// <summary>
    /// Maps a Domain model to a **new** Persistence entity.
    /// Note: Use this primarily for creating new records. Updating existing
    /// entities often requires fetching the tracked entity first and then
    /// applying changes from the domain model.
    /// </summary>
    /// <param name="domain">The domain model.</param>
    /// <returns>A new persistence entity, or null if the domain model was null.</returns>
    TEntity? ToNewEntity(TModel? model);

    /// <summary>
    /// Applies changes from a Domain model onto an existing, tracked Persistence entity.
    /// This is typically used for updates. Implementations should handle mapping properties
    /// and potentially complex relationship updates.
    /// </summary>
    /// <param name="domain">The domain model containing updated state.</param>
    /// <param name="entity">The existing persistence entity (tracked by DbContext).</param>
    void ApplyUpdate(TModel model, TEntity entity);
}
