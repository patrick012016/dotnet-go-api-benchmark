using System.Text.Json.Serialization;
using Api.Aot.Endpoints.Scenario01.BasicOperation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Aot.Endpoints.Scenario01;

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(GetUserProfileResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class Scenario01JsonContext : JsonSerializerContext
{
}
