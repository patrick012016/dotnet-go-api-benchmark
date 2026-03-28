namespace Api.Jit.Endpoints.Scenario02.ServerlessColdStart;

public readonly record struct PingResponse(
    string Status,
    DateTimeOffset Timestamp
);
