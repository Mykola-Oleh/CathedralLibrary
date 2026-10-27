namespace CathedralLibrary.Models;

public class Copy
{
    public int Id { get; set; }
    public int PublicationId { get; set; }
    public Publication Publication { get; set; } = null!;
    public int CopyNumber { get; set; }
    public int CopyStatusId { get; set; }
    public CopyStatus CopyStatus { get; set; } = null!;
    public ICollection<LibraryRequest> Requests { get; set; } = new List<LibraryRequest>();
}
