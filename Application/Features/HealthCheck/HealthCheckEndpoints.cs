namespace AuthImplementation.Application.Features.HealthCheck;

public static class HealthCheckEndpoints
{
    public static void MapHealthCheckEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapHealthCheckEndpoint();
    }
}