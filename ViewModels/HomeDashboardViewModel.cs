using CathedralLibrary.Models;
namespace CathedralLibrary.ViewModels;

public sealed class HomeDashboardViewModel
{
    public int Publications { get; set; }
    public int Authors { get; set; }
    public int Readers { get; set; }
    public int Copies { get; set; }
    public int Requests { get; set; }
    public List<Publication> RecentPublications { get; set; } = new();
}
