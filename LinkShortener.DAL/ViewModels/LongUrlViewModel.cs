using System.ComponentModel.DataAnnotations;

namespace LinkShortener.DAL.ViewModels;

public class LongUrlViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите ссылку")]
    [Url(ErrorMessage = "Ссылка не валидна")]
    public string? LongUrl { get; set; }
    public string? ShortUrl { get; set; }
    public DateTime DateCreate { get; set; }
    public int CountClick { get; set; }
}