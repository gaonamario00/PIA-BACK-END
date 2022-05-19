using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class Premio
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string descripcion { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public Boolean disponible { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public int rifaId { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public Rifa rifa { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public int orden { get; set; }
    }
}
