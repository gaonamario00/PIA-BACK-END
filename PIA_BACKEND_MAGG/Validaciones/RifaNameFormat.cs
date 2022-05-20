using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PIA_BACKEND_MAGG.Validaciones
{
    public class RifaNameFormat: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            Regex regex = new Regex("[a-zA-Z]{4}?[0-9]{1}?");

            bool isValid = regex.IsMatch(value.ToString());

            if (!isValid)
            {
                return new ValidationResult("El formato del nombre de la rifa debe ser {Nombre}{Numero(s)}");
            }

            return ValidationResult.Success;
        }
    }
}
