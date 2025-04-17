using Kwotty.Application.Contracts; // Required for CancellationToken
using Kwotty.Application.Dtos.Quotes;
using Microsoft.AspNetCore.Mvc; // Required for [FromBody], Route constants

namespace Kwotty.Web.EndPoints; // Adjust namespace as needed

public static class QuoteEndpoints
{
    private const string ApiBaseRoute = "/api";
    private const string QuotesRoute = $"{ApiBaseRoute}/quotes";
    private const string QuoteByIdRoute = $"{QuotesRoute}/{{quoteId:guid}}"; // Route for getting a quote by ID

    public static IEndpointRouteBuilder MapQuoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(QuotesRoute)
                       .WithTags("Quotes"); // Optional: Group endpoints in Swagger UI

        // POST /api/quotes
        group.MapPost("", CreateQuoteAsync)
            .WithName("CreateQuote") // Name for link generation
            .Produces<Guid>(StatusCodes.Status201Created) // Success response (body contains ID)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Invalid input
            .ProducesProblem(StatusCodes.Status404NotFound) // Related entity not found
            .ProducesProblem(StatusCodes.Status500InternalServerError); // Other errors

        // GET /api/quotes/{quoteId} - Define the endpoint needed for link generation
        group.MapGet(QuoteByIdRoute, GetQuoteByIdAsync)
             .WithName("GetQuoteById") // Essential for linkGenerator.GetUriByName
             .Produces<Kwotty.Domain.Models.Quote>(StatusCodes.Status200OK) // Assuming returning domain model
             .Produces(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status500InternalServerError);


        // Add other quote endpoints here (GET all, PUT, DELETE) within the group
        // group.MapGet("", GetAllQuotesAsync);
        // group.MapPut(QuoteByIdRoute, UpdateQuoteAsync); // Example
        // group.MapDelete(QuoteByIdRoute, DeleteQuoteAsync); // Example

        return app; // Return app for chaining if needed elsewhere
    }


    private static async Task<IResult> CreateQuoteAsync(
        [FromBody] CreateQuoteDto createQuoteDto,
        IQuoteService quoteService, // Service injected by DI
        LinkGenerator linkGenerator, // Used to generate Location header
        HttpContext httpContext, // Used to generate Location header
        CancellationToken cancellationToken)
    {
        var result = await quoteService.CreateQuoteAsync(createQuoteDto, "eric@ericjansen.com", cancellationToken);

        // Use the ToHttpResult extension method
        string? locationUri = null;
        if (result.IsSuccess)
        {
            // Generate location only on success and if the ID is valid
            locationUri = linkGenerator.GetUriByName(httpContext, "GetQuoteById", new { quoteId = result.Value });
        }

        // Map Result<Guid> to IResult using the extension method
        return result.ToHttpResult(locationUri);
    }

    // --- Implementation for GetQuoteById endpoint needed for Location header ---
    private static async Task<IResult> GetQuoteByIdAsync(
        Guid quoteId,
        IQuoteService quoteService,
        CancellationToken cancellationToken)
    {
        var result = await quoteService.GetQuoteByIdAsync(quoteId, cancellationToken);

        // Use the ToHttpResult extension method for Result<T>
        // Location URI is null for GET requests like this
        return result.ToHttpResult();
    }

    // Implement handlers for other endpoints (GetAll, Update, Delete) similarly,
    // using the appropriate ToHttpResult extension method.
}