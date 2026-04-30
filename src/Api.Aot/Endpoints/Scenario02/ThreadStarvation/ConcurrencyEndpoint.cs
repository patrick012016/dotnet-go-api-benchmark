using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Aot.Endpoints.Scenario02.ThreadStarvation;

public static class ConcurrencyEndpoint
{
    public static void MapConcurrencyTasks(this RouteGroupBuilder group)
    {
        group.MapGet("/concurrent-tasks", HandleAsync)
            .WithName("StarvationConcurrentTasks")
            .Produces<StarvationResponse>();
    }

    internal static async Task<Results<Ok<StarvationResponse>, ProblemHttpResult>> HandleAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        var tasks = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            tasks[i] = SimulateExternalCallAsync(ct);
        }

        await Task.WhenAll(tasks);
        sw.Stop();

        return TypedResults.Ok(new StarvationResponse("Success", tasks.Length, sw.ElapsedMilliseconds));
    }


    private static async Task SimulateExternalCallAsync(CancellationToken ct)
    {
        await Task.Delay(70, ct);
    }
}
