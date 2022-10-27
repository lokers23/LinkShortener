using System.ComponentModel.DataAnnotations;

namespace LinkShortener.DAL.ViewModels;

public partial class LongUrlViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Введите ссылку")]
    [Url(ErrorMessage = "Ссылка не валидна")]
    public string? LongUrl { get; set; }
    public string? ShortUrl { get; set; }
    
    [ValidateDate(ErrorMessage = "Укажите сегодняшнюю дату или раньше")]
    [DataType(DataType.Date, ErrorMessage = "Введите дату")]
    public DateTime DateCreate { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Количество кликов должно быть больше или равно нулю")]
    public int CountClick { get; set; }
}