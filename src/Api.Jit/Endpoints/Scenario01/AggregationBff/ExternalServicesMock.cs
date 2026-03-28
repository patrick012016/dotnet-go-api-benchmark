namespace Api.Jit.Endpoints.Scenario01.AggregationBff;

public static class ExternalServicesMock
{
    public static async Task<ExternalUserDto?> GetUserAsync(int userId, CancellationToken ct)
    {
        await Task.Delay(30, ct);

        if (userId == 999) return null;

        return new ExternalUserDto(userId, "Anna", "Nowak", $"anna.nowak.{userId}@example.com", true);
    }

    public static async Task<List<ExternalOrderDto>> GetOrdersAsync(int userId, CancellationToken ct)
    {
        await Task.Delay(50, ct);

        return
        [
            new ExternalOrderDto(1, 150.00m, "COMPLETED", DateTimeOffset.UtcNow.AddDays(-10)),
            new ExternalOrderDto(2, 350.50m, "COMPLETED", DateTimeOffset.UtcNow.AddDays(-2)),
            new ExternalOrderDto(3, 99.99m, "PROCESSING", DateTimeOffset.UtcNow)
        ];
    }
}
