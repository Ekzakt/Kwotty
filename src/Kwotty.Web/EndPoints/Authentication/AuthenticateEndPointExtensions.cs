namespace Kwotty.Web.EndPoints.Authentication;

public static class AuthenticateEndPointExtensions
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthenticate();

        return app;
    }
}
