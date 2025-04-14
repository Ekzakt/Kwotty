namespace Kwotty.Web.EndPoints.Authentication;

public static class AuthenticateEndPoint
{
    public const string Name = "Authenticate";

    public static IEndpointRouteBuilder MapAuthenticate(this IEndpointRouteBuilder app)
    {
        app.MapPost("authenticate", async (CancellationToken cancellationToken) =>
        {
            return TypedResults.Ok();
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
