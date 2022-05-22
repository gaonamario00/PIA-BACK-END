namespace PIA_BACKEND_MAGG.DTOs.PremiosDTO
{
    public class GetPremioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string descripcion { get; set; }
        public Boolean disponible { get; set; }
        public int orden { get; set; }
    }
}
