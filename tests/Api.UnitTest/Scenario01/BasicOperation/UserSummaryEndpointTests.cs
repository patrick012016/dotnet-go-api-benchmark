using System.Diagnostics.CodeAnalysis;
using Api.Aot.Endpoints.Scenario01.AggregationBff;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.UnitTest.Scenario01.BasicOperation;

[ExcludeFromCodeCoverage]
public class UserSummaryEndpointTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(-5143)]
    public async Task UserSummary_WhenIdIsInvalid_ShouldReturnBadRequest(int invalidId)
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await UserSummaryEndpoint.UserSummaryAsync(invalidId, cts.Token);

        // Assert
        var badRequest = Assert.IsType<BadRequest<ProblemDetails>>(result.Result);
        Assert.NotNull(badRequest.Value);
        Assert.Equal("Invalid ID", badRequest.Value.Title);
        Assert.Equal("User ID must be > 0.", badRequest.Value.Detail);
    }

    [Fact]
    public async Task UserSummary_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        int notFoundId = 999;
        using var cts = new CancellationTokenSource();

        // Act
        var result = await UserSummaryEndpoint.UserSummaryAsync(notFoundId, cts.Token);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task UserSummary_WhenIdIsValid_ShouldReturnOkWithVIPSegment()
    {
        // Arrange
        int validUserId = 42;
        using var cts = new CancellationTokenSource();

        // Act
        var result = await UserSummaryEndpoint.UserSummaryAsync(validUserId, cts.Token);

        // Assert
        var okResult = Assert.IsType<Ok<UserSummaryResponse>>(result.Result);
        var response = okResult.Value;

        Assert.NotNull(response);
        Assert.Equal(validUserId, response.UserId);
        Assert.Equal("Anna Nowak", response.FullName);
        Assert.Equal($"anna.nowak.{validUserId}@example.com", response.Email);
        Assert.Equal("VIP", response.CustomerSegment);
    }
}
