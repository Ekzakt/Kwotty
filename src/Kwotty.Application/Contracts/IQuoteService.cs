using Ardalis.Result;
using Kwotty.Application.Dtos.Quotes;
using Kwotty.Domain.Models;

namespace Kwotty.Application.Contracts;

public interface IQuoteService
{
    /// <summary>
    /// Gets a specific quote by its ID.
    /// </summary>
    Task<Result<Quote>> GetQuoteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all quotes. Consider pagination.
    /// </summary>
    Task<Result<List<Quote>>> GetAllQuotesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all quotes authored by a specific author.
    /// </summary>
    Task<Result<List<Quote>>> GetQuotesByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new quote based on the provided DTO data.
    /// </summary>
    /// <param name="dto">Data Transfer Object containing quote creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the ID of the newly created quote on success.</returns>
    Task<Result<Guid>> CreateQuoteAsync(CreateQuoteDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the visibility of a quote.
    /// </summary>
    /// <param name="quoteId">The ID of the quote to update.</param>
    /// <param name="dto">DTO containing the new visibility state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure (e.g., NotFound).</returns>
    Task<Result> UpdateQuoteVisibilityAsync(Guid quoteId, UpdateQuoteVisibilityDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a quote.
    /// </summary>
    /// <param name="quoteId">The ID of the quote to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure (e.g., NotFound).</returns>
    Task<Result> DeleteQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates a rating for a quote by a user.
    /// </summary>
    /// <param name="quoteId">The ID of the quote being rated.</param>
    /// <param name="dto">DTO containing rating details (UserId, Value).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> RateQuoteAsync(Guid quoteId, RateQuoteDto dto, CancellationToken cancellationToken = default);
}
