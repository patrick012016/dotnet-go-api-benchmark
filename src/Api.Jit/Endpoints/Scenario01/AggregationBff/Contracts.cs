namespace Api.Jit.Endpoints.Scenario01.AggregationBff;

public record ExternalUserDto(int Id, string FirstName, string LastName, string Email, bool IsActive);

public record ExternalOrderDto(int OrderId, decimal Amount, string Status, DateTimeOffset OrderDate);

public record UserSummaryResponse(
    int UserId,
    string FullName,
    string Email,
    string CustomerSegment,
    int TotalOrders,
    decimal TotalSpent
);
