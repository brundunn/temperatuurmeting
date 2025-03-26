using System.Collections.Generic;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Core.Interfaces
{
    // Implementation interface
    public interface ISensorDataFormatter
    {
        string FormatSensorData(SensorData data);
        string FormatSensorList(IEnumerable<SensorData> dataList);
        string FormatStatistics(Dictionary<string, double> statistics);
        string FormatAlert(string alertMessage, string severity);
    }
}