using MovieApi.Dtos;
using MovieApi.Models;
using MovieApi.Repositories;

namespace MovieApi.Endpoints;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this WebApplication app)
    {
        app.MapGet("/api/movies", async (MovieRepository repository) =>
        {
            var movies = await repository.GetMoviesAsync();
            var dtos = movies.Select(m => new MovieDto(m.Id, m.Title, m.Director, m.ReleaseYear));
            return Results.Ok(dtos);
        });

        app.MapGet("/api/movies/{id:int}", async (int id, MovieRepository repository) =>
        {
            var movie = await repository.GetMovieByIdAsync(id);
            return movie is not null
                ? Results.Ok(new MovieDto(movie.Id, movie.Title, movie.Director, movie.ReleaseYear))
                : Results.NotFound();
        });

        app.MapPost("/api/movies", async (CreateMovieRequest request, MovieRepository repository) =>
        {
            var created = await repository.CreateMovieAsync(request);
            var dto = new MovieDto(created.Id, created.Title, created.Director, created.ReleaseYear);
            return Results.Created($"/api/movies/{dto.Id}", dto);
        });

        app.MapPut("/api/movies/{id:int}", async (int id, UpdateMovieRequest request, MovieRepository repository) =>
        {
            var updated = await repository.UpdateMovieAsync(id, request);
            return updated is not null
                ? Results.Ok(new MovieDto(updated.Id, updated.Title, updated.Director, updated.ReleaseYear))
                : Results.NotFound();
        });

        app.MapDelete("/api/movies/{id:int}", async (int id, MovieRepository repository) =>
        {
            var deleted = await repository.DeleteMovieAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}