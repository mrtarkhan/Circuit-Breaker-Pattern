using CircuitBreakerServer;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IRemoteServiceCaller, RemoteServiceCaller>("bare", options =>
    {
        options.BaseAddress = new Uri("http://localhost:5216/");
        options.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddResilienceHandler("circuit-breaker", builder =>
    {
        builder.AddCircuitBreaker(new()
        {
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>().Handle<HttpRequestException>(),
            MinimumThroughput = 2,
            SamplingDuration = TimeSpan.FromSeconds(15), // The time period over which the failure-success ratio is calculated.
            BreakDuration = TimeSpan.FromSeconds(15), // Defines a fixed time period for which the circuit will remain broken/open before attempting to reset.
            FailureRatio = 0.1, // The failure-success ratio that will cause the circuit to break/open. 0.1 means 10% failed of all sampled executions.
        });
    })
    .SelectPipelineByAuthority();

// builder.Services.AddKeyedScoped<IRemoteServiceCaller, RemoteServiceCaller>("bare", (provider, o) =>
// {
//     HttpClient httpClient = new()
//     {
//         BaseAddress = new Uri("http://localhost:5216/"),
//         Timeout = TimeSpan.FromSeconds(10)
//     };
//     return new RemoteServiceCaller(httpClient);
// });

builder.Services.AddKeyedScoped<IRemoteServiceCaller, RemoteServiceCallerWithCircuitBreaker>("cs");
builder.Services.AddKeyedScoped<IRemoteServiceCaller, RemoteServiceCallerWithPolly>("polly");

builder.Services.AddSingleton<CircuitManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/invoke-circuit-breaker", ([FromKeyedServices("polly")] IRemoteServiceCaller remoteServiceCaller) => remoteServiceCaller.CallInvokeOnRemoteServer())
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}