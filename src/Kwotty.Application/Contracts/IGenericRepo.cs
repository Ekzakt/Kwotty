using System.Linq.Expressions;

namespace Kwotty.Application.Contracts;

/// <summary>
/// Defines generic repository operations for domain entities.
/// </summary>
/// <typeparam name="TDomain">The domain model type this repository works with.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public interface IGenericRepo<TDomain, TId> where TDomain : class
{
    /// <summary>
    /// Gets a domain entity by its identifier asynchronously.
    /// Implementations should consider including necessary related data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The domain entity or null if not found.</returns>
    Task<TDomain?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all domain entities asynchronously.
    /// Implementations should consider performance for large datasets.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all domain entities.</returns>
    Task<List<TDomain>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds domain entities based on a predicate asynchronously.
    /// WARNING: Direct translation of domain predicates to efficient database queries can be complex.
    /// Consider specific repository methods or Specification pattern for optimized queries.
    /// </summary>
    /// <param name="predicate">The search predicate based on the DOMAIN model.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching domain entities.</returns>
    Task<List<TDomain>> FindAsync(Expression<Func<TDomain, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new domain entity asynchronously.
    /// </summary>
    /// <param name="domainEntity">The domain entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added domain entity (potentially with updated state like generated IDs if mapping handles it).</returns>
    Task<TDomain> AddAsync(TDomain domainEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing domain entity asynchronously.
    /// </summary>
    /// <param name="domainEntity">The domain entity with updated state. Must have its identifier set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task UpdateAsync(TDomain domainEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a domain entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an entity with the given identifier exists.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}