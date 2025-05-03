using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using AuthImplementation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AuthImplementation.Application.Features.HealthCheck;

public static class HealthCheckEndpoint
{
    public static void MapHealthCheckEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/health", async (AppDbContext dbContext) =>
            {
                var dbIsHealthy = await CheckDatabaseHealthAsync(dbContext);

                if (!dbIsHealthy)
                {
                    var error = new ApiError
                    {
                        Code = ErrorCode.ServiceUnavailable,
                        Message = "Database is unavailable."
                    };

                    return ErrorMapper.ToApiResult(error);
                }

                var result = Result<HealthCheckResult>.Success(new HealthCheckResult());
                return ResultMapper.ToHttpResult(result);
            })
            .WithName("HealthCheck")
            .WithTags("Health")
            .Produces<ApiResponse<HealthCheckResult>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<bool> CheckDatabaseHealthAsync(AppDbContext dbContext)
    {
        try
        {
            await dbContext.Database.OpenConnectionAsync();
            await dbContext.Database.CloseConnectionAsync();
            return true;
        }
        catch (DbException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

    public class HealthCheckResult
    {
    }
}