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
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones de uno a uno
            modelBuilder.Entity<Voluntario>()
                .HasOne(v => v.VoluntarioAdministrativo)
                .WithOne(va => va.voluntario)
                .HasForeignKey<VoluntarioAdministrativo>(va => va.VoluntarioAdministrativoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voluntario>()
               .HasOne(v => v.VoluntarioSanitario)
               .WithOne(va => va.Voluntario)
               .HasForeignKey<VoluntarioSanitario>(va => va.VoluntarioSanitarioId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.VoluntarioAdministrativo)
                .WithOne(va => va.Usuario)
                .HasForeignKey<VoluntarioAdministrativo>(va => va.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.VoluntarioSanitario)
                .WithOne(vs => vs.Usuario)
                .HasForeignKey<VoluntarioSanitario>(vs => vs.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.VoluntarioMision)
                .WithOne(vm => vm.Usuario)
                .HasForeignKey<VoluntarioMision>(vm => vm.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Socio)
                .WithOne(s => s.Usuario)
                .HasForeignKey<Socio>(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones de uno a muchos
            modelBuilder.Entity<Voluntario>()
                .HasOne(v => v.Sede)
                .WithMany(s => s.Voluntarios)
                .HasForeignKey(v => v.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Alimento>()
                .HasOne(a => a.Envio)
                .WithMany(e => e.Alimentos)
                .HasForeignKey(a => a.EnvioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Medicamento>()
                .HasOne(m => m.Envio)
                .WithMany(e => e.Medicamentos)
                .HasForeignKey(m => m.EnvioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MisionHumanitaria>()
                .HasOne(m => m.Envio)
                .WithMany(e => e.MisionesHumanitarias)
                .HasForeignKey(m => m.EnvioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Socio>()
                .HasOne(s => s.TipoCuota)
                .WithMany(tc => tc.Socios)
                .HasForeignKey(s => s.TipoCuotaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Socio>()
                .HasOne(s => s.Sede)
                .WithMany(sd => sd.Socios)
                .HasForeignKey(s => s.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones de muchos a muchos
            modelBuilder.Entity<EnvioSede>()
                .HasKey(es => new { es.EnvioId, es.SedeId });

            modelBuilder.Entity<EnvioSede>()
                .HasOne(es => es.Envio)
                .WithMany(e => e.EnvioSedes)
                .HasForeignKey(es => es.EnvioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EnvioSede>()
                .HasOne(es => es.Sede)
                .WithMany(s => s.EnvioSedes)
                .HasForeignKey(es => es.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VoluntarioMision>()
                .HasKey(vm => new { vm.VoluntarioSanitarioId, vm.MisionId });

            modelBuilder.Entity<VoluntarioMision>()
                .HasOne(vm => vm.VoluntarioSanitario)
                .WithMany(vs => vs.VoluntarioMisiones)
                .HasForeignKey(vm => vm.VoluntarioSanitarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VoluntarioMision>()
                .HasOne(vm => vm.MisionHumanitaria)
                .WithMany(mh => mh.VoluntarioMisiones)
                .HasForeignKey(vm => vm.MisionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
