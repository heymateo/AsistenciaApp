using System.Text;
using AsistenciaApp.Core.Contracts.Services;
using AsistenciaApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AsistenciaApp.Core.Services;

public class DataService : IDataService
{
    private readonly AssistanceDbContext _dbContext;
    public DataService(AssistanceDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));  // Ensure it isn't null
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
        if (_dbContext == null)
        {
            throw new InvalidOperationException("DbContext is not initialized.");
        }

        return await _dbContext.Estudiante.ToListAsync();
    }
}
