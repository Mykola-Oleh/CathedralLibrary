using System.ComponentModel.DataAnnotations;

namespace CathedralLibrary.Models;

public class Publication : EntityBase
{
    [Required, StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [StringLength(3000)]
    public string? Annotation { get; set; }

    [Range(1450, 2100)]
    public int Year { get; set; }

    [StringLength(500)]
    public string? CoverUrl { get; set; }

    public int PublicationTypeId { get; set; }
    public PublicationType PublicationType { get; set; } = null!;
    public ICollection<PublicationAuthor> PublicationAuthors { get; set; } = new List<PublicationAuthor>();
    public ICollection<Copy> Copies { get; set; } = new List<Copy>();
    public ICollection<FundChange> FundChanges { get; set; } = new List<FundChange>();
}
