using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs.PremiosDTO
{
    public class PremioCreacionDTO : IValidatableObject
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string descripcion { get; set; }
        [Required]
        public int rifaId { get; set; }
        public double valorado { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!double.IsNaN(valorado))
            {
                var valor = valorado;

                if (valor < 2000)
                {
                    yield return new ValidationResult("El premio debe de estar valorado en 2k o mas pesos mexicanos", new string[] { nameof(valor) });
                }
            }
        }
    }
}
