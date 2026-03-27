using Api.Aot.Endpoints.Scenario01;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, Scenario01JsonContext.Default);
});
builder.WebHost.UseKestrelHttpsConfiguration();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScenario01();





app.Run();
