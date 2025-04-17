using Ardalis.Result;
using IResult = Microsoft.AspNetCore.Http.IResult;
namespace Kwotty.Web.EndPoints;

public static class ResultExtensions
{
    /// <summary>
    /// Maps an Ardalis.Result<T> to a Microsoft.AspNetCore.Http.IResult.
    /// </summary>
    public static IResult ToHttpResult<T>(this Result<T> result, string? createdLocationUri = null)
    {
        return result.Status switch
        {
            // Success: Return 201 if location is provided, otherwise 200 OK
            ResultStatus.Ok => createdLocationUri != null
                                ? Results.Created(createdLocationUri, result.Value)
                                : Results.Ok(result.Value),

            ResultStatus.NotFound => Results.NotFound(result.Errors.FirstOrDefault() ?? "Resource not found."), // Return 404

            ResultStatus.Invalid => Results.ValidationProblem( // Return 400
                                        errors: result.ValidationErrors.ToDictionary(
                                            e => e.Identifier ?? "_error",
                                            e => new[] { e.ErrorMessage ?? "Validation Error" })),

            ResultStatus.Error => Results.Problem( // Return 500
                                        detail: result.Errors.FirstOrDefault() ?? "An error occurred.",
                                        statusCode: StatusCodes.Status500InternalServerError),

            ResultStatus.Unauthorized => Results.Unauthorized(), // Return 401
            ResultStatus.Forbidden => Results.Forbid(), // Return 403

            // Catch-all for unexpected statuses
            _ => Results.Problem(
                    detail: $"An unexpected result status occurred: {result.Status}.",
                    statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Maps a non-generic Ardalis.Result to a Microsoft.AspNetCore.Http.IResult.
    /// Useful for commands like Update or Delete.
    /// </summary>
    public static IResult ToHttpResult(this Result result)
    {
        return result.Status switch
        {
            // Success: Return 200 OK or 204 No Content (often preferred for successful updates/deletes)
            ResultStatus.Ok => Results.Ok(), // Or Results.NoContent()

            ResultStatus.NotFound => Results.NotFound(result.Errors.FirstOrDefault() ?? "Resource not found."), // 404
            ResultStatus.Invalid => Results.ValidationProblem( // 400
                                        errors: result.ValidationErrors.ToDictionary(
                                            e => e.Identifier ?? "_error",
                                            e => new[] { e.ErrorMessage ?? "Validation Error" })),

            ResultStatus.Error => Results.Problem( // 500
                                        detail: result.Errors.FirstOrDefault() ?? "An error occurred.",
                                        statusCode: StatusCodes.Status500InternalServerError),

            ResultStatus.Unauthorized => Results.Unauthorized(), // 401
            ResultStatus.Forbidden => Results.Forbid(), // 403

            _ => Results.Problem(
                    detail: $"An unexpected result status occurred: {result.Status}.",
                    statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}