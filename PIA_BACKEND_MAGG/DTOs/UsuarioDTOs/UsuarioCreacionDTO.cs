using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PIA_BACKEND_MAGG.DTOs
{
    public class UsuarioCreacionDTO : IValidatableObject
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [EmailAddress]
        public string EmailConfirmed { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [MinLength(6)]
        public string PasswordConfirmed { get; set; }
        [Required]
        [StringLength(maximumLength:10, ErrorMessage = "El telefono no puede contener mas de 10 caracteres")]
        public string PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                Regex regex = new Regex("[0-9]{10}");

                bool isValid = regex.IsMatch(PhoneNumber);

                if (!isValid)
                {
                    yield return new ValidationResult("El numero de telefono debe contener 10 numeros exactos", new string[] { nameof(PhoneNumber) });
                }
            }
        }
    }
}
