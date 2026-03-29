using Api.Aot.Endpoints.Scenario01;
using Api.Aot.Endpoints.Scenario02;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, Scenario01JsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, Scenario02JsonContext.Default);
});

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScenario01();
app.MapScenario02();





app.Run();
