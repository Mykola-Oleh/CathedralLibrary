using System.ComponentModel.DataAnnotations;

namespace CathedralLibrary.Models;

public class FundChange
{
    public int Id { get; set; }
    public int PublicationId { get; set; }
    public Publication Publication { get; set; } = null!;

    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public int CopiesDelta { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
