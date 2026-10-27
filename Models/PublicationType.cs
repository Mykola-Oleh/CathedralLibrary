using System.ComponentModel.DataAnnotations;

namespace CathedralLibrary.Models;

public class PublicationType
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Publication> Publications { get; set; } = new List<Publication>();
}
