using MovieApi.Endpoints;
using MovieApi.Repositories;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MovieRepository>();

var app = builder.Build();

string connectionString = app.Configuration.GetConnectionString("MovieDatabase")!;

app.MapGet("/", () => "Movie and Chill");

app.MapGet("/api/health/db", async (ILogger<Program> logger) =>
{
    try
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand("SELECT 1;", connection);
        object? result = await command.ExecuteScalarAsync();

        return Results.Ok(new
        {
            message = "Database connection successful",
            result
        });
    }
    catch (MySqlException ex)
    {
        logger.LogError(ex, "Database connection failed");
        return Results.Problem(title: "Database unavailable", statusCode: 503);
    }
});

app.MapMovieEndpoints();

app.Run();