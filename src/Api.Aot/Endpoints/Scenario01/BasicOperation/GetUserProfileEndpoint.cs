using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Aot.Endpoints.Scenario01.BasicOperation;

public static class GetUserProfileEndpoint
{
    public static void MapGetUserProfile(this RouteGroupBuilder group)
    {
        var endpointGroup = group.MapGroup("/user-profile");

        endpointGroup.MapGet("/{id:int}", GetUserProfileAsync)
            .WithName("GetUserProfile")
            .Produces<GetUserProfileResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal static async Task<Results<Ok<GetUserProfileResponse>, NotFound, BadRequest<ProblemDetails>>>
        GetUserProfileAsync(
            int id,
            CancellationToken ct)
    {
        if (id <= 0)
        {
            var problem = new ProblemDetails
            {
                Title = "Validation error.",
                Detail = "The user ID must be greater than zero.",
                Status = StatusCodes.Status400BadRequest
            };

            return TypedResults.BadRequest(problem);
        }

        if (id == 999)
        {
            return TypedResults.NotFound();
        }

        await Task.Delay(20, ct);

        var user = new GetUserProfileResponse(
            Id: id,
            FirstName: "John",
            LastName: "Doe",
            Email: $"john.doe.{id}@example.com",
            Role: "Admin",
            CreatedAt: DateTimeOffset.UtcNow
        );

        return TypedResults.Ok(user);
    }
}
