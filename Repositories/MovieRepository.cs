using MovieApi.Models;
using MySqlConnector;

namespace MovieApi.Repositories;

public class MovieRepository
{
private readonly string _connectionString;

public MovieRepository(IConfiguration configuration)
{
    _connectionString = configuration.GetConnectionString("MovieDatabase")!;
}

    public async Task<List<Movie>> GetMoviesAsync()
    {
        List<Movie> movies = [];

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "SELECT id, title, director, release_year FROM movies;",
            connection
        );

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Movie movie = new(
                reader.GetInt32("id"),
                reader.GetString("title"),
                reader.GetString("director"),
                reader.GetInt32("release_year")
            );
            movies.Add(movie);
        }

        return movies;
    }

    public async Task<Movie?> GetMovieByIdAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "SELECT id, title, director, release_year FROM movies WHERE id = @id;",
            connection
        );
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Movie(
                reader.GetInt32("id"),
                reader.GetString("title"),
                reader.GetString("director"),
                reader.GetInt32("release_year")
            );
        }

        return null;
    }

    public async Task<Movie> CreateMovieAsync(CreateMovieRequest request)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            INSERT INTO movies (title, director, release_year)
            VALUES (@title, @director, @releaseYear);
            SELECT LAST_INSERT_ID();
            """,
            connection
        );
        command.Parameters.AddWithValue("@title", request.Title);
        command.Parameters.AddWithValue("@director", request.Director);
        command.Parameters.AddWithValue("@releaseYear", request.ReleaseYear);

        var newId = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new Movie(newId, request.Title, request.Director, request.ReleaseYear);
    }

    public async Task<Movie?> UpdateMovieAsync(int id, UpdateMovieRequest request)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            UPDATE movies
            SET title = @title, director = @director, release_year = @releaseYear
            WHERE id = @id;
            """,
            connection
        );
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@title", request.Title);
        command.Parameters.AddWithValue("@director", request.Director);
        command.Parameters.AddWithValue("@releaseYear", request.ReleaseYear);

        int rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
        {
            return null;
        }

        return new Movie(id, request.Title, request.Director, request.ReleaseYear);
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "DELETE FROM movies WHERE id = @id;",
            connection
        );
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}