using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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
