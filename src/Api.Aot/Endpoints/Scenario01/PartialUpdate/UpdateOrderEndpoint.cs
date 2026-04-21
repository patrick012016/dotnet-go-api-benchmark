using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Aot.Endpoints.Scenario01.PartialUpdate;

public static class UpdateOrderEndpoint
{
    public static void MapPartialUpdate(this RouteGroupBuilder group)
    {
        var endpointGroup = group.MapGroup("/orders");

        endpointGroup.MapPatch("/{id:int}", UpdateOrderAsync)
            .WithName("PartialUpdateOrder")
            .Accepts<UpdateOrderSwaggerSchema>("application/json")
            .Produces<OrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal static async Task<Results<Ok<OrderResponse>, NotFound, BadRequest<ProblemDetails>, ValidationProblem>>
        UpdateOrderAsync(
            int id,
            [FromBody] JsonObject? payload,
            CancellationToken ct)
    {
        if (payload is null)
            return TypedResults.BadRequest(CreateProblem("Missing Body", "JSON payload is required."));

        if (id <= 0)
            return TypedResults.BadRequest(CreateProblem("Invalid ID", "ID must be > 0."));

        if (id == 999)
            return TypedResults.NotFound();

        var currentStatus = "PENDING";
        var currentTotal = 150.00m;
        var isModified = false;

        var errors = new Dictionary<string, string[]>();

        if (payload.TryGetPropertyValue("status", out var statusNode))
        {
            if (statusNode is null)
            {
                errors.Add("status", ["Status cannot be null."]);
            }
            else if (statusNode.AsValue().TryGetValue<string>(out var parsedStatus))
            {
                if (string.IsNullOrWhiteSpace(parsedStatus))
                {
                    errors.Add("status", ["Status cannot be empty."]);
                }
                else
                {
                    currentStatus = parsedStatus;
                    isModified = true;
                }
            }
            else
            {
                errors.Add("status", ["Field must be a string."]);
            }
        }

        if (payload.TryGetPropertyValue("totalValue", out var totalNode))
        {
            if (totalNode is null)
            {
                errors.Add("totalValue", ["Value cannot be null."]);
            }
            else if (totalNode.AsValue().TryGetValue<decimal>(out var parsedTotal))
            {
                currentTotal = parsedTotal;
                isModified = true;
            }
            else
            {
                errors.Add("totalValue", ["Field must be a valid decimal number."]);
            }
        }

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors, title: "Data validation error.");
        }

        if (isModified)
        {
            await Task.Delay(20, ct);
        }

        var response = new OrderResponse(id, currentStatus, currentTotal, DateTimeOffset.UtcNow);
        return TypedResults.Ok(response);
    }

    private static ProblemDetails CreateProblem(string title, string detail) =>
        new() { Title = title, Detail = detail, Status = StatusCodes.Status400BadRequest };
}
