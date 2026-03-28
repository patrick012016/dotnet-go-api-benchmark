using Api.Jit.Endpoints.Scenario02.ServerlessColdStart;

namespace Api.Jit.Endpoints.Scenario02;

public static class Scenario02Module
{
    public static void MapScenario02(this IEndpointRouteBuilder app)
    {
        var scenarioGroup = app.MapGroup("/scenario-02")
            .WithTags("Scenario 02: Serverless case test");


        scenarioGroup.MapPing();
    }
}
