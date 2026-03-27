namespace Api.Jit.Endpoints.Scenario01.PartialUpdate;

public readonly record struct OrderResponse(
    int Id,
    string Status,
    decimal TotalValue,
    DateTimeOffset UpdatedAt
);

public readonly record struct UpdateOrderSwaggerSchema(
    string? Status,
    decimal? TotalValue
);
