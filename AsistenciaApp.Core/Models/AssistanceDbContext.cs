using Microsoft.EntityFrameworkCore;

namespace AsistenciaApp.Core.Models;

public class AssistanceDbContext : DbContext
{
    public DbSet<Estudiante> Estudiante { get; set; }
    public DbSet<Registro_Asistencia> Registro_Asistencia { get; set; }
    public DbSet<Centro_Educativo> Centro_Educativo { get; set; }
    public DbSet<Admin> Admin { get; set; }
    public AssistanceDbContext(DbContextOptions<AssistanceDbContext> options)
        : base(options) // Pass the options to the base DbContext constructor
    {
    }

    public AssistanceDbContext()
    {
    }

    // USAR UNA DIRECCION DINAMICA, NO DIRECCION LOCAL PERO NO FUNCIONA
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ruta fija a la base de datos en el escritorio
        var dbPath = @"C:\Users\mateo\Desktop\Assistance\Assistance\DB_ASSISTANCE.db";

        // Configurar SQLite con la ruta fija
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure entity mappings, relationships, etc.
    }
}
