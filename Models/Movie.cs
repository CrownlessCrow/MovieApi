namespace MovieApi.Models;

public record Movie(int Id, string Title, string Director, int ReleaseYear);

public record CreateMovieRequest(string Title, string Director, int ReleaseYear);

public record UpdateMovieRequest(string Title, string Director, int ReleaseYear);