using System.ComponentModel.DataAnnotations;

namespace CathedralLibrary.ViewModels;

public class AuthorForm
{
    [Required(ErrorMessage = "Вкажіть ім’я автора"), StringLength(160)]
    public string FullName { get; set; } = "";
    [StringLength(1000)] public string? Biography { get; set; }
}
public class ReaderForm
{
    [Required(ErrorMessage = "Вкажіть ПІБ"), StringLength(160)] public string FullName { get; set; } = "";
    [StringLength(120)] public string Faculty { get; set; } = "";
    [StringLength(120)] public string Department { get; set; } = "";
    [StringLength(100)] public string Position { get; set; } = "";
    [EmailAddress(ErrorMessage = "Некоректна електронна адреса"), StringLength(254)] public string? Email { get; set; }
}
public class PublicationForm
{
    [Required(ErrorMessage = "Вкажіть назву"), StringLength(250)] public string Title { get; set; } = "";
    [StringLength(3000)] public string? Annotation { get; set; }
    [Range(1450, 2100, ErrorMessage = "Рік має бути від 1450 до 2100")] public int Year { get; set; } = DateTime.UtcNow.Year;
    [Url(ErrorMessage = "Вкажіть повне URL-посилання"), StringLength(500)] public string? CoverUrl { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть тип видання")] public int PublicationTypeId { get; set; }
    public List<int> AuthorIds { get; set; } = new();
}
public class CopyForm
{
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть видання")] public int PublicationId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Номер має бути додатним")] public int CopyNumber { get; set; } = 1;
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть стан")] public int CopyStatusId { get; set; }
}
public class RequestForm
{
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть читача")] public int ReaderId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть примірник")] public int CopyId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть статус")] public int RequestStatusId { get; set; }
    [DataType(DataType.Date)] public DateTime? ReturnedAt { get; set; }
}
