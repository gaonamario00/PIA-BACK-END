﻿namespace PIA_BACKEND_MAGG.Entidades
{
    public class Rifa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double costo { get; set; }
        public string userId { get; set; }
        public List<ParticipanteRifa> participaciones { get; set; }
        public List<Premio> premios { get; set; }
    }
}