using AsistenciaApp.Core.Models;

namespace AsistenciaApp.Core.Contracts.Services;
public interface IDataService
{
    T Read<T>(string folderPath, string fileName);

    void Save<T>(string folderPath, string fileName, T content);

    void Delete(string folderPath, string fileName);
    Task<IEnumerable<Estudiante>> GetGridDataAsync();
    Task SaveCentroEducativoAsync(string folderPath, string fileName, Centro_Educativo centroEducativo);

    Task<Centro_Educativo> GetCentroEducativoAsync();
}
