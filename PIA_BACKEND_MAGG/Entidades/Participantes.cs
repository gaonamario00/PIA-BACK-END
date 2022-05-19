﻿using Microsoft.AspNetCore.Identity;
using PIA_BACKEND_MAGG.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class Participantes
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        [UserNameFormat]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string IdUser { get; set; }
        public IdentityUser user { get; set; }
        public List<ParticipanteRifa> Participaciones { get; set; }
    }
}
