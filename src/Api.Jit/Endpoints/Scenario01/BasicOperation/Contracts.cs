namespace Api.Jit.Endpoints.Scenario01.BasicOperation;

public readonly record struct GetUserProfileResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    DateTimeOffset CreatedAt
);
