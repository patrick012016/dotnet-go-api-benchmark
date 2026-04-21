using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Api.Aot.Endpoints.Scenario01.PartialUpdate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.UnitTest.Scenario01.BasicOperation;

[ExcludeFromCodeCoverage]
public class UpdateOrderEndpointTests
{
    [Fact]
    public async Task UpdateOrder_WhenPayloadIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await UpdateOrderEndpoint.UpdateOrderAsync(10, null, cts.Token);

        // Assert
        var badRequest = Assert.IsType<BadRequest<ProblemDetails>>(result.Result);
        Assert.NotNull(badRequest.Value);
        Assert.Equal("Missing Body", badRequest.Value.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(-5143)]
    public async Task UpdateOrder_WhenIdIsInvalid_ShouldReturnBadRequest(int invalidId)
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await UpdateOrderEndpoint.UpdateOrderAsync(invalidId, new JsonObject(), cts.Token);

        // Assert
        var badRequest = Assert.IsType<BadRequest<ProblemDetails>>(result.Result);
        Assert.NotNull(badRequest.Value);
        Assert.Equal("Invalid ID", badRequest.Value.Title);
    }

    [Fact]
    public async Task UpdateOrder_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        int notFoundId = 999;

        // Act
        var result = await UpdateOrderEndpoint.UpdateOrderAsync(notFoundId, new JsonObject(), cts.Token);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    // Method providing data for testing method
    public static TheoryData<JsonObject, string> InvalidPayloadsData()
    {
        var data = new TheoryData<JsonObject, string>();

        data.Add(new JsonObject { ["status"] = string.Empty, ["totalValue"] = 122m }, "status");
        data.Add(new JsonObject { ["status"] = "SHIPPED", ["totalValue"] = null }, "totalValue");
        data.Add(new JsonObject { ["status"] = "SHIPPED", ["totalValue"] = "test" }, "totalValue");
        data.Add(new JsonObject { ["status"] = 122, ["totalValue"] = 12m }, "status");

        // Testing cases with multiple errors at once
        data.Add(new JsonObject { ["status"] = null, ["totalValue"] = "not_a_number" }, "status");
        data.Add(new JsonObject { ["status"] = null, ["totalValue"] = "not_a_number" }, "totalValue");

        return data;
    }

    [Theory]
    [MemberData(nameof(InvalidPayloadsData))]
    public async Task UpdateOrder_WhenPayloadIsInvalid_ShouldReturnValidationProblem(JsonObject payload,
        string expectedErrorField)
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await UpdateOrderEndpoint.UpdateOrderAsync(10, payload, cts.Token);

        // Assert
        var validationProblem = Assert.IsType<ValidationProblem>(result.Result);
        Assert.True(validationProblem.ProblemDetails.Errors.ContainsKey(expectedErrorField));
    }

    [Fact]
    public async Task UpdateOrder_WhenOnlyStatusIsUpdated_ShouldReturnOkWithUpdatedStatus()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var payload = new JsonObject { ["status"] = "SHIPPED" };

        // Act
        var result = await UpdateOrderEndpoint.UpdateOrderAsync(10, payload, cts.Token);

        // Assert
        var okResult = Assert.IsType<Ok<OrderResponse>>(result.Result);
        var response = okResult.Value;

        Assert.Equal(10, response.Id);
        Assert.Equal("SHIPPED", response.Status);
        Assert.Equal(150.00m, response.TotalValue);
    }

    [Fact]
    public async Task UpdateOrder_WhenBothFieldsAreUpdated_ShouldReturnOkWithBothNewValues()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var payload = new JsonObject
        {
            ["status"] = "COMPLETED",
            ["totalValue"] = 499.99m
        };

        // Act
        var result = await UpdateOrderEndpoint.UpdateOrderAsync(42, payload, cts.Token);

        // Assert
        var okResult = Assert.IsType<Ok<OrderResponse>>(result.Result);
        var response = okResult.Value;

        Assert.Equal(42, response.Id);
        Assert.Equal("COMPLETED", response.Status);
        Assert.Equal(499.99m, response.TotalValue);
    }
}
