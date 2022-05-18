﻿using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class Premio
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string descripcion { get; set; }
        [Required]
        public Boolean disponible { get; set; }
        [Required]
        public int rifaId { get; set; }
        [Required]
        public Rifa rifa { get; set; }
        public int orden { get; set; }
    }
}
