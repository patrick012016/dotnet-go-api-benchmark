using Api.Jit.Endpoints.Scenario01.BasicOperation;

namespace Api.Jit.Endpoints.Scenario01;

public static class Scenario01Module
{
    public static void MapScenario01(this IEndpointRouteBuilder app)
    {
        var scenarioGroup = app.MapGroup("/scenario-01")
            .WithTags("Scenario 01: Basic business logic");

        scenarioGroup.MapGetUserProfile();
    }
}
