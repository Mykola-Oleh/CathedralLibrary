using System.ComponentModel.DataAnnotations;

namespace CathedralLibrary.Models;

public class CopyStatus
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Copy> Copies { get; set; } = new List<Copy>();
}
