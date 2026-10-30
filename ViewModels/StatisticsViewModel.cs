namespace CathedralLibrary.ViewModels;

public sealed class StatisticsViewModel
{
    public int Publications { get; set; }
    public int Authors { get; set; }
    public int Readers { get; set; }
    public int Copies { get; set; }
    public List<string> TypeNames { get; set; } = new();
    public List<int> TypeCounts { get; set; } = new();
    public List<string> StatusNames { get; set; } = new();
    public List<int> StatusCounts { get; set; } = new();
}
