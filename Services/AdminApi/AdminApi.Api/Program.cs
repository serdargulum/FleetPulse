using Microsoft.EntityFrameworkCore;
using AdminApi.Infrastructure.Persistence;
using AdminApi.Infrastructure.Persistence.Repositories;
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Application.Commands.Auth.Register;
using AdminApi.Application.Common.Security;
using AdminApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using AdminApi.Domain.Entities;
using AdminApi.Api.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using AdminApi.Application.Contracts.Services;
using AdminApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection connection string is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, o =>
    {
        o.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
        o.UseNetTopologySuite();
    });
});

builder.Services
    .AddHealthChecks()
    .AddNpgSql(connectionString);

builder.Services
    .AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "auth_token";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", limiter =>
    {
        limiter.PermitLimit = 100;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITelemetryRecordRepository, TelemetryRecordRepository>();
builder.Services.AddScoped<ITelemetryAlertRepository, TelemetryAlertRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddControllers();
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

var app = builder.Build();

app.MapHealthChecks("/health");

app.UseCors("Frontend");

app.UseRateLimiter();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        if (!await dbContext.Database.CanConnectAsync())
        {
            throw new Exception("Cannot connect to PostgreSQL.");
        }

        await dbContext.Database.MigrateAsync();
        DatabaseSeeder.Seed(dbContext);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Database initialization failed: {ex.GetBaseException().Message}");
        Environment.ExitCode = 1;
    }
}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("", () =>
{
    return Results.Json(new { result = "ok" });
});

app.Run();