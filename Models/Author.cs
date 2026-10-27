using System.ComponentModel.DataAnnotations;

namespace CathedralLibrary.Models;

public class Author : EntityBase
{
    [Required, StringLength(160)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Biography { get; set; }

    public ICollection<PublicationAuthor> PublicationAuthors { get; set; } = new List<PublicationAuthor>();
}
