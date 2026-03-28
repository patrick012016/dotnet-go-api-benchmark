using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Jit.Endpoints.Scenario02.ServerlessColdStart;

public static class PingEndpoint
{
    public static void MapPing(this RouteGroupBuilder group)
    {
        group.MapGet("/ping", Results<Ok<PingResponse>, ProblemHttpResult> () =>
            {
                var response = new PingResponse("pong", DateTimeOffset.UtcNow);
                return TypedResults.Ok(response);
            })
            .WithName("PingColdStart")
            .Produces<PingResponse>();
    }
}
