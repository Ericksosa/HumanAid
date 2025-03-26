using HumanAid.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanAid.Data
{
    public class HumanAidDbContext : DbContext
    {
        public HumanAidDbContext(DbContextOptions<HumanAidDbContext> options)
              : base(options)
        {

        }

        public DbSet<Envio> Envio { get; set; }
        public DbSet<Alimento> Alimento { get; set; }
        public DbSet<Medicamento> Medicamento { get; set; }
        public DbSet<EnvioSede> EnvioSede { get; set; }
        public DbSet<MisionHumanitaria> MisionHumanitaria { get; set; }
        public DbSet<Sede> Sede { get; set; }
        public DbSet<Socio> Socio { get; set; }
        public DbSet<TipoCuota> TipoCuota { get; set; }
        public DbSet<Voluntario> Voluntario { get; set; }
        public DbSet<VoluntarioAdministrativo> VoluntarioAdministrativo { get; set; }
        public DbSet<VoluntarioMision> VoluntarioMision { get; set; }
        public DbSet<VoluntarioSanitario> VoluntarioSanitario { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Relaciones de uno a uno
            modelBuilder.Entity<Voluntario>()
                .HasOne(v => v.VoluntarioAdministrativo)
                .WithOne(va => va.voluntario)
                .HasForeignKey<VoluntarioAdministrativo>(va => va.VoluntarioAdministrativoId);

            modelBuilder.Entity<Voluntario>()
               .HasOne(v => v.VoluntarioSanitario)
               .WithOne(va => va.Voluntario)
               .HasForeignKey<VoluntarioSanitario>(va => va.VoluntarioSanitarioId);

            //Relaciones de uno a mucho
            modelBuilder.Entity<Voluntario>()
                .HasOne(v => v.Sede)
                .WithMany(s => s.Voluntarios)
                .HasForeignKey(v => v.SedeId);

            modelBuilder.Entity<Alimento>()
                .HasOne(v => v.Envio)
                .WithMany(s => s.Alimentos)
                .HasForeignKey(v => v.EnvioId);

            modelBuilder.Entity<Medicamento>()
                .HasOne(v => v.Envio)
                .WithMany(s => s.Medicamentos)
                .HasForeignKey(v => v.EnvioId);

            modelBuilder.Entity<MisionHumanitaria>()
                .HasOne(v => v.Envio)
                .WithMany(s => s.MisionesHumanitarias)
                .HasForeignKey(v => v.EnvioId);

            modelBuilder.Entity<Socio>()
                .HasOne(v => v.TipoCuota)
                .WithMany(s => s.Socios)
                .HasForeignKey(v => v.TipoCuotaId);


            modelBuilder.Entity<Socio>()
                .HasOne(s => s.Sede)
                .WithMany(s => s.Socios)
                .HasForeignKey(s => s.SedeId);

            //Relaciones de mucho a mucho
            modelBuilder.Entity<EnvioSede>()
        .HasKey(ea => new { ea.EnvioId, ea.SedeId });

            modelBuilder.Entity<EnvioSede>()
                .HasOne(ea => ea.Envio)
                .WithMany(e => e.EnvioSedes)
                .HasForeignKey(ea => ea.EnvioId);

            modelBuilder.Entity<EnvioSede>()
                .HasOne(ea => ea.Sede)
                .WithMany(a => a.EnvioSedes)
                .HasForeignKey(ea => ea.SedeId);

            modelBuilder.Entity<VoluntarioMision>()
        .HasKey(ea => new { ea.VoluntarioSanitarioId, ea.MisionId });

            modelBuilder.Entity<VoluntarioMision>()
                .HasOne(ea => ea.VoluntarioSanitario)
                .WithMany(e => e.VoluntarioMisiones)
                .HasForeignKey(ea => ea.VoluntarioSanitarioId);

            modelBuilder.Entity<VoluntarioMision>()
                .HasOne(ea => ea.MisionHumanitaria)
                .WithMany(a => a.VoluntarioMisiones)
                .HasForeignKey(ea => ea.MisionId);
        }
    }
}
