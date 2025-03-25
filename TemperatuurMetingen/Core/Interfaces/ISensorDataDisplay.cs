using System.Collections.Generic;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Core.Interfaces
{
    // Abstraction interface
    public interface ISensorDataDisplay
    {
        void DisplaySensorData(SensorData data);
        void DisplaySensorList(IEnumerable<SensorData> dataList);
        void DisplayStatistics(Dictionary<string, double> statistics, string title);
        void DisplayAlert(string alertMessage, string severity);
    }
}