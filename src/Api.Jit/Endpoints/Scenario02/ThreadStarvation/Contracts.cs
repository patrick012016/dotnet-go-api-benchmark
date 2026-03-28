namespace Api.Jit.Endpoints.Scenario02.ThreadStarvation;

public readonly record struct StarvationResponse(
    string Status,
    int TasksCompleted,
    long ElapsedMilliseconds
);
