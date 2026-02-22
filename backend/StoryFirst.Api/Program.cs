using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StoryFirst.Api.Middleware;
using StoryFirst.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add Authentication
var keycloakAuthority = builder.Configuration["Authentication:Keycloak:Authority"]
    ?? throw new InvalidOperationException("Keycloak Authority is not configured");
var keycloakAudience = builder.Configuration["Authentication:Keycloak:Audience"]
    ?? throw new InvalidOperationException("Keycloak Audience is not configured");
var keycloakMetadataAddress = builder.Configuration["Authentication:Keycloak:MetadataAddress"];
var keycloakInternalUrl = builder.Configuration["Authentication:Keycloak:InternalUrl"];
var validIssuers = builder.Configuration.GetSection("Authentication:Keycloak:ValidIssuers").Get<string[]>()
    ?? new[] { keycloakAuthority };

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.Audience = keycloakAudience;
        options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Authentication:Keycloak:RequireHttpsMetadata");

        if (!string.IsNullOrEmpty(keycloakMetadataAddress))
        {
            options.MetadataAddress = keycloakMetadataAddress;
        }

        // When the backend can't reach the Authority URL directly (e.g. inside Docker),
        // use a backchannel handler that rewrites requests to the internal Keycloak URL.
        // This ensures JWKS and other metadata endpoints are fetched from the reachable address.
        if (!string.IsNullOrEmpty(keycloakInternalUrl))
        {
            options.BackchannelHttpHandler = new KeycloakBackchannelHandler(keycloakInternalUrl, validIssuers);
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = builder.Configuration.GetValue<bool>("Authentication:Keycloak:ValidateIssuer"),
            ValidateAudience = builder.Configuration.GetValue<bool>("Authentication:Keycloak:ValidateAudience"),
            ValidateLifetime = builder.Configuration.GetValue<bool>("Authentication:Keycloak:ValidateLifetime"),
            ValidIssuers = validIssuers,
            ValidAudiences = new[] { keycloakAudience, "account" }
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Token received, Authority: {Authority}, MetadataAddress: {MetadataAddress}",
                    options.Authority, options.MetadataAddress);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Add Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddScoped<IEpicRepository, EpicRepository>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<ISpikeRepository, SpikeRepository>();
builder.Services.AddScoped<IExternalEntityRepository, ExternalEntityRepository>();

// Register Services
builder.Services.AddScoped<StoryFirst.Api.Areas.ProjectManagement.Services.IProjectService, StoryFirst.Api.Areas.ProjectManagement.Services.ProjectService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.IThemeService, StoryFirst.Api.Areas.UserStoryMapping.Services.ThemeService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.IEpicService, StoryFirst.Api.Areas.UserStoryMapping.Services.EpicService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.IStoryService, StoryFirst.Api.Areas.UserStoryMapping.Services.StoryService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.ISpikeService, StoryFirst.Api.Areas.UserStoryMapping.Services.SpikeService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.ITaskService, StoryFirst.Api.Areas.UserStoryMapping.Services.TaskService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.IThemeService, StoryFirst.Api.Areas.UserStoryMapping.Services.ThemeService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.IEpicService, StoryFirst.Api.Areas.UserStoryMapping.Services.EpicService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.IStoryService, StoryFirst.Api.Areas.UserStoryMapping.Services.StoryService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.ISpikeService, StoryFirst.Api.Areas.UserStoryMapping.Services.SpikeService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.UserStoryMapping.Services.ITaskService, StoryFirst.Api.Areas.UserStoryMapping.Services.TaskService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.SprintPlanning.Services.IBacklogService, StoryFirst.Api.Areas.SprintPlanning.Services.BacklogService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.SprintPlanning.Services.ISprintService, StoryFirst.Api.Areas.SprintPlanning.Services.SprintService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.SprintPlanning.Services.ITeamService, StoryFirst.Api.Areas.SprintPlanning.Services.TeamService>();
builder.Services.AddScoped<StoryFirst.Api.Areas.SprintPlanning.Services.IReleaseService, StoryFirst.Api.Areas.SprintPlanning.Services.ReleaseService>();

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        DbSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Use global exception handler
app.UseGlobalExceptionHandler();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

/// <summary>
/// HTTP handler that rewrites outgoing requests from any public-facing Keycloak URL
/// to the internal Docker-network URL. This allows the JWT middleware to fetch OIDC
/// metadata and JWKS signing keys even when the Authority URL isn't directly reachable.
/// Handles the case where Keycloak metadata returns URLs with a different origin than
/// the configured Authority (e.g. jwks_uri at localhost:8080 vs Authority at localhost:8180).
/// </summary>
class KeycloakBackchannelHandler : DelegatingHandler
{
    private readonly string _internalOrigin;
    private readonly string[] _externalOrigins;

    public KeycloakBackchannelHandler(string internalUrl, string[] validIssuers)
        : base(new HttpClientHandler())
    {
        _internalOrigin = internalUrl.TrimEnd('/');

        // Build a set of external origins to rewrite from the valid issuers.
        // Each issuer looks like "http://localhost:8180/realms/storyfirst", so extract the origin part.
        _externalOrigins = validIssuers
            .Select(issuer =>
            {
                try { return new Uri(issuer).GetLeftPart(UriPartial.Authority); }
                catch { return null; }
            })
            .Where(origin => origin != null && !origin.Equals(_internalOrigin, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()!;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri != null)
        {
            var original = request.RequestUri.ToString();
            foreach (var externalOrigin in _externalOrigins)
            {
                if (original.StartsWith(externalOrigin!, StringComparison.OrdinalIgnoreCase))
                {
                    var rewritten = _internalOrigin + original[externalOrigin!.Length..];
                    request.RequestUri = new Uri(rewritten);
                    break;
                }
            }
        }
        return base.SendAsync(request, cancellationToken);
    }
}
