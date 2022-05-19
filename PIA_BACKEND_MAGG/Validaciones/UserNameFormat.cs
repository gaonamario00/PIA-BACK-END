using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PIA_BACKEND_MAGG.Validaciones
{
    public class UserNameFormat: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            Regex regex = new Regex("[a-zA-Z]{4}?_[0-9]{2}?");

            bool isValid = regex.IsMatch(value.ToString());

            if (!isValid)
            {
                return new ValidationResult("El formato de UserName debe ser {Nombre}_{Numeros}");
            }

            return ValidationResult.Success;
        }
    }
}
