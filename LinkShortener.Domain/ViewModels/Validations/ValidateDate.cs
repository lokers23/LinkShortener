using System.ComponentModel.DataAnnotations;

namespace LinkShortener.Domain.ViewModels;

public partial class LongUrlViewModel
{
    public class ValidateDate : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? objValue, ValidationContext validationContext)
        {
            var dateValue = objValue as DateTime? ?? new DateTime();
            
            if (dateValue.Date > DateTime.Now.Date)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            
            return ValidationResult.Success;
        }
    }
}