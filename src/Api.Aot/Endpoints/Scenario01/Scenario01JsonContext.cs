using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Api.Aot.Endpoints.Scenario01.AggregationBff;
using Api.Aot.Endpoints.Scenario01.BasicOperation;
using Api.Aot.Endpoints.Scenario01.PartialUpdate;
using Microsoft.AspNetCore.Mvc;

namespace Api.Aot.Endpoints.Scenario01;

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(GetUserProfileResponse))]

[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(OrderResponse))]
[JsonSerializable(typeof(HttpValidationProblemDetails))]

[JsonSerializable(typeof(UserSummaryResponse))]

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class Scenario01JsonContext : JsonSerializerContext
{
}
