using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Jit.Endpoints.Scenario01.AggregationBff;

public static class UserSummaryEndpoint
{
    public static void MapBffAggregation(this RouteGroupBuilder group)
    {
        group.MapGet("/summary/{userId:int}", UserSummaryAsync)
            .WithName("BffAggregationSummary");
    }

    internal static async Task<Results<Ok<UserSummaryResponse>, NotFound, BadRequest<ProblemDetails>>> UserSummaryAsync(
        int userId,
        CancellationToken ct)
    {
        if (userId <= 0)
            return TypedResults.BadRequest(CreateProblem("Invalid ID", "User ID must be > 0."));

        var userTask = ExternalServicesMock.GetUserAsync(userId, ct);
        var ordersTask = ExternalServicesMock.GetOrdersAsync(userId, ct);

        await Task.WhenAll(userTask, ordersTask);

        var user = userTask.Result;
        var orders = ordersTask.Result;

        if (user is null || !user.IsActive)
        {
            return TypedResults.NotFound();
        }

        var totalSpent = orders.Sum(o => o.Amount);
        var totalOrders = orders.Count;

        var customerSegment = totalSpent > 500 ? "VIP" : "REGULAR";

        var response = new UserSummaryResponse(
            UserId: user.Id,
            FullName: $"{user.FirstName} {user.LastName}",
            Email: user.Email,
            CustomerSegment: customerSegment,
            TotalOrders: totalOrders,
            TotalSpent: totalSpent
        );

        return TypedResults.Ok(response);
    }

    private static ProblemDetails CreateProblem(string title, string detail) =>
        new() { Title = title, Detail = detail, Status = StatusCodes.Status400BadRequest };
}
