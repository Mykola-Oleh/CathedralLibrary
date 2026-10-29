namespace CathedralLibrary.Api;

public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Skip, int Limit, int Total, string? NextLink);
