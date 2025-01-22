using Microsoft.UI.Xaml.Data;
using System;

namespace AsistenciaApp.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                // Formatear la fecha según el formato deseado
                return dateTime.ToString("dd/MM/yyyy");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException(); // No necesitamos convertir de vuelta
        }
    }
}
