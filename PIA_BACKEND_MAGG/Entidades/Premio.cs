namespace PIA_BACKEND_MAGG.Entidades
{
    public class Premio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string descripcion { get; set; }
        public Boolean disponible { get; set; }
        public int rifaId { get; set; }
        public Rifa rifa { get; set; }
    }
}
