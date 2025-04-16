using Bogus;
using HumanAid.Data;

namespace HumanAid.Models
{
    public class FakeDataSeeder
    {
        public static void Seed(HumanAidDbContext context)
        {
            if (context.Envio.Any() || context.Sede.Any() || context.Socio.Any() || context.Voluntario.Any() || context.Usuario.Any() || context.Medicamento.Any() || context.Alimento.Any() || context.EnvioSede.Any() || context.MisionHumanitaria.Any() || context.VoluntarioAdministrativo.Any() || context.VoluntarioSanitario.Any() || context.VoluntarioMision.Any())
            {
                return;
            }

            // Rol
            var roles = new List<Rol>
            {
                new Rol { Nombre = "Administrador", FechaRegistro = DateTime.Now, Descripcion = "Este es el administrador" },
                new Rol { Nombre = "Socio", FechaRegistro = DateTime.Now, Descripcion = "Este es el Socio" },
                new Rol { Nombre = "VoluntarioAdministrativo", FechaRegistro = DateTime.Now, Descripcion = "Este es el VoluntarioAdministrativo" },
                new Rol { Nombre = "VoluntarioSanitario", FechaRegistro = DateTime.Now, Descripcion = "Este es el VoluntarioSanitario" }
            };

            context.Rol.AddRange(roles);
            context.SaveChanges();

            // Usuario
            var usuarioFaker = new Faker<Usuario>()
                .RuleFor(u => u.Correo, f => f.Internet.Email())
                .RuleFor(u => u.Clave, f => f.Internet.Password())
                .RuleFor(u => u.RolId, f => f.PickRandom(roles).RolId)
                .RuleFor(u => u.FechaRegistro, f => f.Date.Recent());

            var usuarios = usuarioFaker.Generate(10);
            context.Usuario.AddRange(usuarios);
            context.SaveChanges();
            var usuariosGuardados = context.Usuario.ToList();

            // Sede
            var sedeFaker = new Faker<Sede>()
                .RuleFor(u => u.Nombre, f => f.Company.CompanyName())
                .RuleFor(u => u.Ciudad, f => f.Address.City())
                .RuleFor(u => u.Direccion, f => f.Address.StreetAddress())
                .RuleFor(u => u.Director, f => f.Name.FullName())
                .RuleFor(u => u.FechaFundacion, f => f.Date.Past(50));

            var sedes = sedeFaker.Generate(10);
            context.Sede.AddRange(sedes);
            context.SaveChanges();
            var sedesGuardadas = context.Sede.ToList();

            // TipoCuota
            var tipoCuotas = new List<TipoCuota>
            {
                new TipoCuota { Nombre = "Mínima", Importe = 10, Descripcion = "La moneda es el euro" },
                new TipoCuota { Nombre = "Media", Importe = 20, Descripcion = "La moneda es el euro" },
                new TipoCuota { Nombre = "Máxima", Importe = 30, Descripcion = "La moneda es el euro" },
            };

            context.TipoCuota.AddRange(tipoCuotas);
            context.SaveChanges();
            var tipoCuotasGuardadas = context.TipoCuota.ToList();

            // Socio
            var socioFaker = new Faker<Socio>()
                .RuleFor(u => u.Nombre, f => f.Name.FullName())
                .RuleFor(u => u.FechaNacimiento, f => f.Date.Past(30))
                .RuleFor(u => u.CuentaBancaria, f => f.Finance.Iban())
                .RuleFor(u => u.FechaPago, f => f.Date.Recent())
                .RuleFor(u => u.TipoCuotaId, f => f.PickRandom(tipoCuotasGuardadas).TipoCuotaId)
                .RuleFor(u => u.SedeId, f => f.PickRandom(sedesGuardadas).SedeId)
                .RuleFor(u => u.UsuarioId, f => f.PickRandom(usuariosGuardados).UsuarioId);

            var socios = socioFaker.Generate(10);
            context.Socio.AddRange(socios);
            context.SaveChanges();

            // Envio
            var envioFaker = new Faker<Envio>()
                .RuleFor(u => u.Fecha, f => f.Date.Past())
                .RuleFor(u => u.Destino, f => f.Address.City())
                .RuleFor(u => u.TipoEnvio, f => f.Commerce.Department())
                .RuleFor(u => u.Estado, f => f.Random.Word())
                .RuleFor(u => u.CodigoEnvio, f => f.Random.AlphaNumeric(10))
                .RuleFor(u => u.FechaSalida, f => f.Date.Future());

            var envios = envioFaker.Generate(10);
            context.Envio.AddRange(envios);
            context.SaveChanges();
            var enviosGuardados = context.Envio.ToList();

            // Voluntario
            var voluntarioFaker = new Faker<Voluntario>()
                .RuleFor(u => u.Nombre, f => f.Name.FullName())
                .RuleFor(u => u.Direccion, f => f.Address.StreetAddress())
                .RuleFor(u => u.FechaNacimiento, f => f.Date.Past(30))
                .RuleFor(u => u.SedeId, f => f.PickRandom(sedesGuardadas).SedeId)
                .RuleFor(u => u.FechaRegistro, f => f.Date.Recent())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Telefono, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(u => u.UsuarioId, f => f.PickRandom(usuariosGuardados).UsuarioId);

            var voluntarios = voluntarioFaker.Generate(10);
            context.Voluntario.AddRange(voluntarios);
            context.SaveChanges();
            var voluntariosGuardados = context.Voluntario.ToList();

            // Medicamento
            var medicamentoFaker = new Faker<Medicamento>()
                .RuleFor(m => m.Nombre, f => f.Commerce.ProductName())
                .RuleFor(m => m.Dosis, f => f.Random.String2(10))
                .RuleFor(m => m.cantidad, f => f.Random.Int(1, 100))
                .RuleFor(m => m.EnvioId, f => f.PickRandom(enviosGuardados).EnvioId);

            var medicamentos = medicamentoFaker.Generate(10);
            context.Medicamento.AddRange(medicamentos);
            context.SaveChanges();

            // Alimento
            var alimentoFaker = new Faker<Alimento>()
                .RuleFor(a => a.Tipo, f => f.Commerce.ProductName())
                .RuleFor(a => a.Peso, f => f.Random.Decimal(1, 100))
                .RuleFor(a => a.EnvioId, f => f.PickRandom(enviosGuardados).EnvioId);

            var alimentos = alimentoFaker.Generate(10);
            context.Alimento.AddRange(alimentos);
            context.SaveChanges();

            // EnvioSede
            var envioSedeFaker = new Faker<EnvioSede>()
                .RuleFor(es => es.EnvioId, f => f.PickRandom(enviosGuardados).EnvioId)
                .RuleFor(es => es.SedeId, f => f.PickRandom(sedesGuardadas).SedeId);

            var envioSedes = new List<EnvioSede>();
            var envioSedeKeys = new HashSet<(int EnvioId, int SedeId)>();

            while (envioSedes.Count < 10)
            {
                var envioSede = envioSedeFaker.Generate();
                var key = (envioSede.EnvioId, envioSede.SedeId);

                if (!envioSedeKeys.Contains(key))
                {
                    envioSedes.Add(envioSede);
                    envioSedeKeys.Add(key);
                }
            }

            context.EnvioSede.AddRange(envioSedes);
            context.SaveChanges();

            // MisionHumanitaria
            var misionHumanitariaFaker = new Faker<MisionHumanitaria>()
                .RuleFor(mh => mh.Fecha, f => f.Date.Past())
                .RuleFor(mh => mh.Descripcion, f => f.Lorem.Sentence())
                .RuleFor(mh => mh.Estado, f => f.Random.Word())
                .RuleFor(mh => mh.EnvioId, f => f.PickRandom(enviosGuardados).EnvioId);

            var misionesHumanitarias = misionHumanitariaFaker.Generate(10);
            context.MisionHumanitaria.AddRange(misionesHumanitarias);
            context.SaveChanges();
            var misionesHumanitariasGuardadas = context.MisionHumanitaria.ToList();

            // VoluntarioAdministrativo
            var voluntarioAdministrativoFaker = new Faker<VoluntarioAdministrativo>()
                .RuleFor(va => va.Profesion, f => f.Name.JobTitle())
                .RuleFor(va => va.Departamento, f => f.Commerce.Department())
                .RuleFor(va => va.VoluntarioId, f => f.PickRandom(voluntariosGuardados).VoluntarioId);

            var voluntariosAdministrativos = voluntarioAdministrativoFaker.Generate(10);
            context.VoluntarioAdministrativo.AddRange(voluntariosAdministrativos);
            context.SaveChanges();

            // VoluntarioSanitario
            var voluntarioSanitarioFaker = new Faker<VoluntarioSanitario>()
                .RuleFor(vs => vs.Profesion, f => f.Name.JobTitle())
                .RuleFor(vs => vs.VoluntarioId, f => f.PickRandom(voluntariosGuardados).VoluntarioId);

            var voluntariosSanitarios = voluntarioSanitarioFaker.Generate(10);
            context.VoluntarioSanitario.AddRange(voluntariosSanitarios);
            context.SaveChanges();
            var voluntariosSanitariosGuardados = context.VoluntarioSanitario.ToList();

            // VoluntarioMision
            var voluntarioMisionFaker = new Faker<VoluntarioMision>()
                .RuleFor(vm => vm.VoluntarioSanitarioId, f => f.PickRandom(voluntariosSanitariosGuardados).VoluntarioSanitarioId)
                .RuleFor(vm => vm.MisionId, f => f.PickRandom(misionesHumanitariasGuardadas).MisionId);

            var voluntariosMisiones = voluntarioMisionFaker.Generate(10);
            context.VoluntarioMision.AddRange(voluntariosMisiones);
            context.SaveChanges();
        }
    }
}
