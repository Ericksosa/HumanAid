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
        public DbSet<Pago> Pago { get; set; }
        public DbSet<Gastos> Gastos { get; set; }

        public object Seguimiento { get; internal set; }
        public IEnumerable<object> Donacion { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones de uno a uno
            modelBuilder.Entity<Voluntario>()
                .HasOne(v => v.VoluntarioAdministrativo)
                .WithOne(va => va.Voluntario)
                .HasForeignKey<VoluntarioAdministrativo>(va => va.VoluntarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voluntario>()
                .HasOne(v => v.VoluntarioSanitario)
                .WithOne(vs => vs.Voluntario)
                .HasForeignKey<VoluntarioSanitario>(vs => vs.VoluntarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voluntario>()
               .HasOne(v => v.Usuario)
               .WithOne(u => u.Voluntario)
               .HasForeignKey<Voluntario>(v => v.UsuarioId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Socio)
                .WithOne(s => s.Usuario)
                .HasForeignKey<Socio>(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Socio)
                .WithMany(s => s.Pagos)
                .HasForeignKey(p => p.SocioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.TipoCuota)
                .WithMany(tc => tc.Pagos)
                .HasForeignKey(p => p.TipoCuotaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gastos>()
                .HasOne(g => g.Sede)
                .WithMany(s => s.Gastos)
                .HasForeignKey(g => g.SedeId)
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
