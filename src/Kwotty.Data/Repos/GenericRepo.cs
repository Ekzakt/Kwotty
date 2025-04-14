using Kwotty.Application.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kwotty.Data.Repos;

/// <summary>
/// Generic repository implementation using EF Core and injected mapping.
/// </summary>
/// <typeparam name="TDomain">The domain model type.</typeparam>
/// <typeparam name="TEntity">The corresponding persistence entity type.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public class GenericRepo<TDomain, TEntity, TId> : IGenericRepo<TDomain, TId>
    where TDomain : class
    where TEntity : class 
    where TId : IEquatable<TId>
{
    protected readonly KwottyDbContext _context;
    protected readonly IMapper<TDomain, TEntity> _mapper;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepo(KwottyDbContext context, IMapper<TDomain, TEntity> mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _dbSet = _context.Set<TEntity>();
    }

    /// <summary>
    /// Gets the underlying IQueryable for the DbSet.
    /// Override in derived classes to add default eager loading (Includes).
    /// Example: protected override IQueryable<Persistence.Quote> DbSetWithIncludes => _dbSet.Include(q => q.Author).Include(q => q.Items);
    /// </summary>
    protected virtual IQueryable<TEntity> DbSetWithIncludes => _dbSet.AsQueryable();

    public virtual async Task<TDomain?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        // Use FindAsync for PK lookup. For includes, override DbSetWithIncludes and use FirstOrDefaultAsync.
        var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
        
        return _mapper.ToModel(entity);
    }

    public virtual async Task<List<TDomain>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Use the potentially overridden DbSetWithIncludes
        var entities = await DbSetWithIncludes.ToListAsync(cancellationToken);
        
        return _mapper.ToModelList(entities).ToList();
    }

    public virtual async Task<List<TDomain>> FindAsync(Expression<Func<TDomain, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // WARNING: Still relies on potentially inefficient in-memory filtering.
        // Override this in specific repositories or use a Specification pattern for efficient DB querying.

        var allDomainEntities = await GetAllAsync(cancellationToken); // This uses the potentially overridden GetAllAsync
        var results = allDomainEntities.Where(predicate.Compile()).ToList();

        return results;
    }

    public virtual async Task<TDomain> AddAsync(TDomain domainEntity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEntity);

        var persistenceEntity = _mapper.ToNewEntity(domainEntity);
        if (persistenceEntity == null)
        {
            throw new InvalidOperationException($"Mapping domain entity type {typeof(TDomain).Name} to persistence entity resulted in null.");
        }

        await _dbSet.AddAsync(persistenceEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.ToModel(persistenceEntity)!;
    }

    public virtual async Task UpdateAsync(TDomain domainEntity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEntity);

        // Assuming TDomain has an 'Id' property of type TId.
        // Use reflection or add an interface constraint (e.g., IEntity<TId>) to TDomain.
        var idProperty = typeof(TDomain).GetProperty("Id");
        if (idProperty == null || idProperty.PropertyType != typeof(TId))
        {
            throw new InvalidOperationException($"Domain entity type {typeof(TDomain).Name} must have an 'Id' property of type {typeof(TId).Name} accessible for updates.");
        }
        var id = (TId?)idProperty.GetValue(domainEntity);
        if (id == null || id.Equals(default(TId))) // Check for default/null ID
        {
            throw new InvalidOperationException($"Domain entity type {typeof(TDomain).Name} must have a valid 'Id' for updates.");
        }


        // 1. Fetch the existing entity from the database
        var existingEntity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        if (existingEntity == null)
        {
            // Log this situation
            throw new KeyNotFoundException($"Entity of type {typeof(TEntity).Name} with ID {id} not found for update.");
        }

        // 2. Use the injected mapper's ApplyUpdate method to transfer changes
        _mapper.ApplyUpdate(domainEntity, existingEntity);

        // 3. Save changes (EF Core tracks changes on 'existingEntity')
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entityToDelete = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);

        if (entityToDelete != null)
        {
            _dbSet.Remove(entityToDelete);
            await _context.SaveChangesAsync(cancellationToken);
        }
        // else: Entity not found. Decide whether to throw or not. Silently succeeding is often acceptable for deletes.
    }

    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);

        return entity != null;
    }
}