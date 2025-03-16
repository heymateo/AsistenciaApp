using Microsoft.EntityFrameworkCore;

namespace AsistenciaApp.Core.Models;

public class AssistanceDbContext : DbContext
{
    public DbSet<Estudiante> Estudiante { get; set; }
    public DbSet<Registro_Asistencia> Registro_Asistencia { get; set; }
    public DbSet<Centro_Educativo> Centro_Educativo { get; set; }
    public DbSet<Admin> Admin { get; set; }
    public AssistanceDbContext(DbContextOptions<AssistanceDbContext> options)
        : base(options)
    {
    }

    public AssistanceDbContext()
    {
    }

    // USAR UNA DIRECCION DINAMICA, NO DIRECCION LOCAL PERO NO FUNCIONA
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = @"C:\Users\mateo\Desktop\Assistance\Assistance\DB_ASSISTANCE.db";

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Registro_Asistencia>()
        .HasOne(r => r.Estudiante)
        .WithMany(e => e.Registro_Asistencia)
        .HasForeignKey(r => r.Id_Estudiante);

        modelBuilder.Entity<Registro_Asistencia>()
           .Property(r => r.Fecha)
           .HasColumnType("date");
    }
}
