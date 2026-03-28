using System.Text.Json.Serialization;
using Api.Aot.Endpoints.Scenario02.ServerlessColdStart;

namespace Api.Aot.Endpoints.Scenario02;

[JsonSerializable(typeof(PingResponse))]

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class Scenario02JsonContext : JsonSerializerContext
{
}
