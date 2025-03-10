﻿using System.Collections.ObjectModel;
using AsistenciaApp.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaApp.ViewModels;

public partial class AsistenciaViewModel : ObservableObject 
{
    [ObservableProperty]
    private ObservableCollection<Registro_Asistencia>? filteredEntries;

    [ObservableProperty]
    private DateTime? targetDate;


    public AsistenciaViewModel()
    {
        TargetDate = DateTime.Now.Date;
        LoadFilteredEntriesAsync();
    }

    private async Task LoadFilteredEntriesAsync()
    {
        if (TargetDate == null) return;

        using var dbContext = new AssistanceDbContext();

        var query = await (from ra in dbContext.Registro_Asistencia
                           join e in dbContext.Estudiante
                           on ra.Id_Estudiante equals e.Id_Estudiante
                           where ra.Fecha == TargetDate.Value
                           select new
                           {
                               ra.Id_Registro,
                               ra.Id_Estudiante,
                               ra.Fecha,
                               ra.Hora_Entrada,
                               ra.Asistio,
                               NombreEstudiante = e.Nombre
                           }).ToListAsync();

        var result = query.Select(item => new Registro_Asistencia
        {
            Id_Registro = item.Id_Registro,
            Id_Estudiante = item.Id_Estudiante,
            Fecha = item.Fecha,
            Hora_Entrada = item.Hora_Entrada,
            Asistio = item.Asistio,
            NombreEstudiante = item.NombreEstudiante
        }).ToList();

        FilteredEntries = new ObservableCollection<Registro_Asistencia>(result);
    }

    partial void OnTargetDateChanged(DateTime? value)
    {
        if (value != null)
        {
            _ = LoadFilteredEntriesAsync();
        }
    }
}
