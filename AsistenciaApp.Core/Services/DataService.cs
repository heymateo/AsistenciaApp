using System.Diagnostics;
using System.Text;
using AsistenciaApp.Core.Contracts.Services;
using AsistenciaApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AsistenciaApp.Core.Services;

public class DataService : IDataService
{
    private readonly IDbContextFactory<AssistanceDbContext> _dbContextFactory;
    public DataService(IDbContextFactory<AssistanceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    }

    public T Read<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        return default;
    }

    public void Save<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }

    public async Task<IEnumerable<Estudiante>> GetGridDataAsync()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Estudiante.ToListAsync();
    }

    public async Task SaveCentroEducativoAsync(string folderPath, string fileName, Centro_Educativo centroEducativo)
    {
        try
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.Centro_Educativo.Add(centroEducativo);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al guardar el centro educativo: {ex.Message}");
        }
    }

    public async Task<Centro_Educativo> GetCentroEducativoAsync()
    {
        try
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Centro_Educativo.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al obtener el Centro Educativo: {ex.Message}");
            return null;
        }
    }
}