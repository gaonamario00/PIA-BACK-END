using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class Premio
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "La descripcion debe de tener maximo 100 caracteres")]
        public string descripcion { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public Boolean disponible { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public int rifaId { get; set; }
        public Rifa rifa { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public int orden { get; set; }
        [NotMapped]
        public double valorado { get; set; }

    }
}
