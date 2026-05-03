using Api.Aot.Endpoints.Scenario02.ThreadStarvation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.UnitTest.Scenario2;

public class ConcurrencyEndpointTests
{
    [Fact]
    public async Task Concurrency_ShouldCompleteFiveTasks_AndRunConcurrently()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await ConcurrencyEndpoint.ConcurrencyAsync(cts.Token);

        // Assert
        var okResult = Assert.IsType<Ok<StarvationResponse>>(result.Result);
        var response = okResult.Value;
        Assert.Equal("Success", response.Status);
        Assert.Equal(5, response.TasksCompleted);
    }
}
