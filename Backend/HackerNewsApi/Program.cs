using HackerNewsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HTTP client & service via dependency injection
builder.Services
    .AddHttpClient<IHackerNewsService, HackerNewsService>(client =>
    {
        client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
        client.Timeout = TimeSpan.FromSeconds(15);
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

// Add memory cache
builder.Services.AddMemoryCache();

// Add CORS for Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // Angular dev server
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
