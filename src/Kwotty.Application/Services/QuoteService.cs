using Ardalis.Result;
using Kwotty.Application.Contracts;
using Kwotty.Application.Dtos.Quotes;
using Kwotty.Domain.Models;

namespace Kwotty.Application.Services;

public class QuoteService : IQuoteService
{
    private readonly IGenericRepo<Quote, Guid> _quoteRepository;
    private readonly IGenericRepo<Author, Guid> _authorRepository;
    private readonly IGenericRepo<User, Guid> _userRepository;
    private readonly IGenericRepo<Category, Guid> _categoryRepository;
    private readonly IGenericRepo<Medium, Guid> _mediumRepository;
    // Inject IUnitOfWork or DbContext directly if transaction management is needed across multiple repo calls

    public QuoteService(
        IGenericRepo<Quote, Guid> quoteRepository,
        IGenericRepo<Author, Guid> authorRepository,
        IGenericRepo<User, Guid> userRepository,
        IGenericRepo<Category, Guid> categoryRepository,
        IGenericRepo<Medium, Guid> mediumRepository)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _mediumRepository = mediumRepository ?? throw new ArgumentNullException(nameof(mediumRepository));
    }

    public async Task<Result<Quote>> GetQuoteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var quote = await _quoteRepository.GetByIdAsync(id, cancellationToken);
        if (quote == null)
        {
            return Result<Quote>.NotFound($"Quote with ID {id} not found.");
        }
        return Result<Quote>.Success(quote);
    }

    public async Task<Result<List<Quote>>> GetAllQuotesAsync(CancellationToken cancellationToken = default)
    {
        var quotes = await _quoteRepository.GetAllAsync(cancellationToken);
        return Result<List<Quote>>.Success(quotes);
    }

    public async Task<Result<List<Quote>>> GetQuotesByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        // Using FindAsync - remember potential performance issue.
        // Consider specific repo method: _quoteRepository.GetByAuthorIdAsync(authorId, cancellationToken);
        var quotes = await _quoteRepository.FindAsync(q => q.Author.Id == authorId, cancellationToken);
        return Result<List<Quote>>.Success(quotes);
    }

    public async Task<Result<Guid>> CreateQuoteAsync(CreateQuoteDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        try
        {
            // 1. Validate input DTO (using FluentValidation or similar is recommended)
            // Basic checks:
            if (dto.AuthorId == Guid.Empty)
            {
                return Result<Guid>.Invalid(new ValidationError { Identifier = nameof(dto.AuthorId), ErrorMessage = "AuthorId is required." });
            }

            // 2. Fetch required related domain entities
            var author = await _authorRepository.GetByIdAsync(dto.AuthorId, cancellationToken);
            if (author == null)
            {
                return Result<Guid>.NotFound($"Author with ID {dto.AuthorId} not found.");
            }

            var categories = new List<Category>();
            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                categories = await _categoryRepository.FindAsync(c => dto.CategoryIds.Contains(c.Id), cancellationToken);
                var foundIds = categories.Select(c => c.Id).ToList();
                var missingIds = dto.CategoryIds.Except(foundIds).ToList();
                if (missingIds.Any())
                    return Result<Guid>.Invalid(new ValidationError { Identifier = nameof(dto.CategoryIds), ErrorMessage = $"Categories not found: {string.Join(", ", missingIds)}" });
            }

            var itemMediums = new Dictionary<Guid, Medium>();
            var requiredMediumIds = dto.quoteItems?
                                       .Select(i => i.MediumId)
                                       .Where(id => id.HasValue)
                                       .Select(id => id!.Value)
                                       .Distinct().ToList() ?? new List<Guid>();
            if (requiredMediumIds.Any())
            {
                var fetchedMedia = await _mediumRepository.FindAsync(m => requiredMediumIds.Contains(m.Id), cancellationToken);
                itemMediums = fetchedMedia.ToDictionary(m => m.Id);
                var missingMediaIds = requiredMediumIds.Except(itemMediums.Keys).ToList();
                if (missingMediaIds.Any())
                    return Result<Guid>.Invalid(new ValidationError { Identifier = nameof(CreateQuoteItemDto.MediumId), ErrorMessage = $"Media items not found: {string.Join(", ", missingMediaIds)}" });
            }

            // 3. Create domain objects
            var quoteId = Guid.NewGuid(); // Generate ID for the new quote

            var quoteItems = dto.quoteItems?.Select(itemDto => new QuoteItem(
                    Guid.NewGuid(), // Generate new ID for each item
                    itemDto.Text,
                    itemDto.Text, // Consider generating slug server-side if needed
                    itemDto.SortOrder,
                    itemDto.MediumId.HasValue && itemMediums.TryGetValue(itemDto.MediumId.Value, out var medium) ? medium : null
                )).ToList();


            // Create the Quote aggregate root using its constructor
            var newQuote = new Quote(
                quoteId,
                author,
                dto.IsHidden,
                DateTimeOffset.UtcNow,
                createdBy,
                quoteItems,
                categories
            );


            // 4. Add to repository
            var addedQuote = await _quoteRepository.AddAsync(newQuote, cancellationToken);

            // 5. Return success result with the new ID
            return Result<Guid>.Success(addedQuote.Id);
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return Result<Guid>.Error($"An error occurred while creating the quote: {ex.Message}");
        }
    }

    public async Task<Result> UpdateQuoteVisibilityAsync(Guid quoteId, UpdateQuoteVisibilityDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (quoteId == Guid.Empty) return Result.Invalid(new ValidationError { Identifier = nameof(quoteId), ErrorMessage = "Quote ID is required." });

        try
        {
            var quote = await _quoteRepository.GetByIdAsync(quoteId, cancellationToken);
            if (quote == null)
            {
                return Result.NotFound($"Quote with ID {quoteId} not found.");
            }

            // Use domain method to update state
            quote.UpdateVisibility(dto.IsHidden); // Assuming this method exists, or use Hide()/Show()

            await _quoteRepository.UpdateAsync(quote, cancellationToken);

            return Result.Success();
        }
        catch (KeyNotFoundException) // Catch specific exception from repo if needed
        {
            return Result.NotFound($"Quote with ID {quoteId} not found during update attempt.");
        }
        catch (Exception ex)
        {
            // Log ex
            return Result.Error($"An error occurred while updating quote visibility: {ex.Message}");
        }
    }

    public async Task<Result> DeleteQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        if (quoteId == Guid.Empty) return Result.Invalid(new ValidationError { Identifier = nameof(quoteId), ErrorMessage = "Quote ID is required." });
        try
        {
            // Check existence first for a clearer NotFound result
            bool exists = await _quoteRepository.ExistsAsync(quoteId, cancellationToken);
            if (!exists) return Result.NotFound($"Quote with ID {quoteId} not found.");

            await _quoteRepository.DeleteAsync(quoteId, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            // Log ex
            return Result.Error($"An error occurred while deleting quote {quoteId}: {ex.Message}");
        }
    }


    public async Task<Result> RateQuoteAsync(Guid quoteId, RateQuoteDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (quoteId == Guid.Empty) return Result.Invalid(new ValidationError { Identifier = nameof(quoteId), ErrorMessage = "Quote ID is required." });
        if (dto.UserId == Guid.Empty) return Result.Invalid(new ValidationError { Identifier = nameof(dto.UserId), ErrorMessage = "User ID is required." });
        // Add validation for dto.RatingValue if needed

        try
        {
            // Fetch aggregates involved
            var quote = await _quoteRepository.GetByIdAsync(quoteId, cancellationToken);
            if (quote == null) return Result.NotFound($"Quote {quoteId} not found.");

            var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
            if (user == null) return Result.NotFound($"User {dto.UserId} not found.");

            // Perform domain operation
            quote.AddOrUpdateRating(user, dto.RatingValue, DateTimeOffset.UtcNow);

            // Persist changes to the Quote aggregate
            await _quoteRepository.UpdateAsync(quote, cancellationToken);

            return Result.Success();
        }
        catch (KeyNotFoundException knfex) // Can be thrown by UpdateAsync if quote deleted between fetch and save
        {
            return Result.NotFound(knfex.Message);
        }
        catch (Exception ex)
        {
            // Log ex
            return Result.Error($"An error occurred while rating quote {quoteId}: {ex.Message}");
        }
    }
}
