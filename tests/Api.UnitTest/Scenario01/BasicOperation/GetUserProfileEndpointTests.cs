using System.Diagnostics.CodeAnalysis;
using Api.Aot.Endpoints.Scenario01.BasicOperation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.UnitTest.Scenario01.BasicOperation;

[ExcludeFromCodeCoverage]
public class GetUserProfileEndpointTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(-5143)]
    public async Task GetUserProfile_WhenIdIsInvalid_ShouldReturnBadRequest(int invalidId)
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await GetUserProfileEndpoint.GetUserProfileAsync(invalidId, cts.Token);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<ProblemDetails>>(result.Result);
        Assert.NotNull(badRequestResult.Value);
        Assert.Equal("Validation error.", badRequestResult.Value.Title);
        Assert.Equal(400, badRequestResult.Value.Status);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        int notFoundId = 999;
        using var cts = new CancellationTokenSource();

        // Act
        var result = await GetUserProfileEndpoint.GetUserProfileAsync(notFoundId, cts.Token);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GetUserProfile_WhenIdIsValid_ShouldReturnOkWithProfile()
    {
        // Arrange
        int validId = 42;
        using var cts = new CancellationTokenSource();

        // Act
        var result = await GetUserProfileEndpoint.GetUserProfileAsync(validId, cts.Token);

        // Assert
        var okResult = Assert.IsType<Ok<GetUserProfileResponse>>(result.Result);

        Assert.Equal(validId, okResult.Value.Id);
        Assert.Equal("John", okResult.Value.FirstName);
        Assert.Equal("Admin", okResult.Value.Role);
        Assert.Equal($"john.doe.{validId}@example.com", okResult.Value.Email);
    }
}
