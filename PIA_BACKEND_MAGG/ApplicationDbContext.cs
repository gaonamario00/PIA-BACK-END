using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.Entidades;

namespace PIA_BACKEND_MAGG
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //}

        public DbSet<Participantes> participantes { get; set; }
        public DbSet<Rifa> rifas { get; set; }
        public DbSet<Premio> premios { get; set; }
        public DbSet<ParticipanteRifa> participantesRifa { get; set; }
        public DbSet<TarjetaGanadora> TarjetasGanadoras { get; set; }
    }
}