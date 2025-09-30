using HackerNews.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddMemoryCache();

builder.Services
    .AddHttpClient<IHackerNewsService, HackerNewsService>(client =>
    {
        client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
        client.Timeout = TimeSpan.FromSeconds(15);
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Build().Run();
