namespace CathedralLibrary.Models;

public class LibraryRequest
{
    public int Id { get; set; }
    public int ReaderId { get; set; }
    public Reader Reader { get; set; } = null!;
    public int CopyId { get; set; }
    public Copy Copy { get; set; } = null!;
    public int RequestStatusId { get; set; }
    public RequestStatus RequestStatus { get; set; } = null!;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnedAt { get; set; }
}
