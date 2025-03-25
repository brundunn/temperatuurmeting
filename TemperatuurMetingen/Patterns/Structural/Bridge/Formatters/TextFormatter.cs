using System;
using System.Collections.Generic;
using System.Text;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Formatters
{
    public class TextFormatter : ISensorDataFormatter
    {
        public string FormatSensorData(SensorData data)
        {
            if (data == null)
                return "No data";
                
            return $"Sensor: {data.SerialNumber} | Type: {data.Type} | " +
                   $"Temp: {data.Temperature:F1}°C | Humidity: {data.Humidity:F1}% | " +
                   $"Battery: {data.BatteryLevel}/{data.BatteryMax} | State: {data.State}";
        }
        
        public string FormatSensorList(IEnumerable<SensorData> dataList)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Sensor Data List:");
            builder.AppendLine("----------------");
            
            int count = 0;
            foreach (var data in dataList)
            {
                builder.AppendLine(FormatSensorData(data));
                count++;
            }
            
            builder.AppendLine("----------------");
            builder.AppendLine($"Total sensors: {count}");
            
            return builder.ToString();
        }
        
        public string FormatStatistics(Dictionary<string, double> statistics)
        {
            var builder = new StringBuilder();
            
            foreach (var stat in statistics)
            {
                string formattedValue = stat.Key.Contains("Temperature") ? $"{stat.Value:F1}°C" :
                                      stat.Key.Contains("Humidity") ? $"{stat.Value:F1}%" :
                                      stat.Key.Contains("Battery") ? $"{stat.Value:F1}%" :
                                      $"{stat.Value:F1}";
                                      
                builder.AppendLine($"{stat.Key}: {formattedValue}");
            }
            
            return builder.ToString();
        }
        
        public string FormatAlert(string alertMessage, string severity)
        {
            return $"[{severity}] {alertMessage}";
        }
    }
}