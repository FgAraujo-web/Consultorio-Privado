

using Consultorio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Consultorio.Infrastructure.Data;
public class ConsultorioDbContext(DbContextOptions<ConsultorioDbContext> options) : DbContext(options) {
 public DbSet<Utente> Utentes => Set<Utente>(); public DbSet<Especialidade> Especialidades => Set<Especialidade>(); public DbSet<Medico> Medicos => Set<Medico>(); public DbSet<TipoAto> TiposAtos => Set<TipoAto>(); public DbSet<Consulta> Consultas => Set<Consulta>(); public DbSet<AtoClinico> AtosClinicos => Set<AtoClinico>(); public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
 public DbSet<HorarioMedico> HorariosMedicos
        => Set<HorarioMedico>();
    public DbSet<IndisponibilidadeMedico> IndisponibilidadesMedicos
        => Set<IndisponibilidadeMedico>();
    protected override void OnModelCreating(ModelBuilder m) {
  m.Entity<Utente>(e => { e.ToTable("Utentes"); e.HasKey(x=>x.UtenteId); });
  m.Entity<Especialidade>(e => { e.ToTable("Especialidades"); e.HasKey(x=>x.EspecialidadeId); });
  m.Entity<Medico>(e => { e.ToTable("Medicos"); e.HasKey(x=>x.MedicoId); });
  m.Entity<TipoAto>(e => { e.ToTable("TiposAtos"); e.HasKey(x=>x.TipoAtoId); });
  m.Entity<Consulta>(e => { e.ToTable("Consultas"); e.HasKey(x=>x.ConsultaId); });
  m.Entity<AtoClinico>(e => { e.ToTable("AtosClinicos"); e.HasKey(x=>x.AtoId); });
  m.Entity<Pagamento>(e => { e.ToTable("Pagamentos"); e.HasKey(x=>x.PagamentoId); });
  m.Entity<Utente>().HasIndex(x=>x.Codigo).IsUnique();
  m.Entity<Medico>().HasIndex(x=>x.CedulaFicticia).IsUnique();
  m.Entity<TipoAto>().HasIndex(x=>x.Codigo).IsUnique();
  m.Entity<Pagamento>().HasIndex(x=>x.ReferenciaFicticia).IsUnique();
  m.Entity<Utente>().Property(x=>x.DataNascimento).HasColumnType("date");
  m.Entity<TipoAto>().Property(x=>x.PrecoBase).HasPrecision(10,2);
  m.Entity<AtoClinico>().Property(x=>x.Valor).HasPrecision(10,2);
  m.Entity<Pagamento>().Property(x=>x.Valor).HasPrecision(10,2);
  m.Entity<Consulta>().Property(x=>x.DataHora).HasColumnType("datetime2(0)");
  m.Entity<AtoClinico>().Property(x=>x.DataAto).HasColumnType("datetime2(0)");
  m.Entity<Pagamento>().Property(x=>x.DataPagamento).HasColumnType("datetime2(0)");
        m.Entity<HorarioMedico>(e =>
        {
            e.ToTable("HorariosMedicos");
            e.HasKey(x => x.HorarioMedicoId);

            e.HasOne(x => x.Medico)
                .WithMany()
                .HasForeignKey(x => x.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new
            {
                x.MedicoId,
                x.DiaSemana,
                x.HoraInicio
            });
        });

        m.Entity<IndisponibilidadeMedico>(e =>
        {
            e.ToTable("IndisponibilidadesMedicos");
            e.HasKey(x => x.IndisponibilidadeMedicoId);

            e.HasOne(x => x.Medico)
                .WithMany()
                .HasForeignKey(x => x.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(x => x.DataHoraInicio)
                .HasColumnType("datetime2(0)");

            e.Property(x => x.DataHoraFim)
                .HasColumnType("datetime2(0)");

            e.HasIndex(x => new
            {
                x.MedicoId,
                x.DataHoraInicio
            });
        });
    }
}
