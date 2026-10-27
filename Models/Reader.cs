using System.ComponentModel.DataAnnotations;

namespace CathedralLibrary.Models;

public class Reader : EntityBase
{
    [Required, StringLength(160)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(120)]
    public string Faculty { get; set; } = string.Empty;

    [StringLength(120)]
    public string Department { get; set; } = string.Empty;

    [StringLength(100)]
    public string Position { get; set; } = string.Empty;

    [EmailAddress, StringLength(254)]
    public string? Email { get; set; }

    public ICollection<LibraryRequest> Requests { get; set; } = new List<LibraryRequest>();
}
